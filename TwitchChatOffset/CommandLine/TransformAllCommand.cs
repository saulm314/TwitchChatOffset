using TwitchChatOffset.ConsoleUtils;
using TwitchChatOffset.Csv;
using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
using TwitchChatOffset.Options.Optimisations;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using CSVFile;

namespace TwitchChatOffset.CommandLine;

public static class TransformAllCommand
{
    public static readonly Command Command = new("transform-all", "Transform all files in a directory whose name matches a search pattern");

    static TransformAllCommand()
    {
        Command.AddOptions<TransformAllCliOptions>();
        Command.SetAction(Execute);
    }

    private static void Execute(ParseResult parseResult)
    {
        TransformAllCliOptions cliOptions = parseResult.ParseOptions<TransformAllCliOptions>();

        string[] inputPaths = Directory.GetFiles(cliOptions.CommonOptions.InputDir, cliOptions.SearchPattern);
        PrintEnumerable(inputPaths, "Input files found:", 0, cliOptions.Quiet);

        List<TransformAllCommonOptions> commonOptionsList;
        if (cliOptions.CsvPath != string.Empty)
        {
            PrintLine("Reading CSV data...", 0, cliOptions.Quiet);
            using CSVReader reader = CSVReader.FromFile(cliOptions.CsvPath, CsvUtils.CsvSettings);
            commonOptionsList = [];
            foreach (TransformAllCommonOptions commonOptions in CsvSerialization.Deserialize<TransformAllCommonOptions>(reader))
                commonOptionsList.Add(BulkTransform.ResolveConflicts(commonOptions, cliOptions.CommonOptions));

            PrintLine("Sorting CSV data...", 0, cliOptions.Quiet);
            commonOptionsList.Sort(TransformAllCommonOptionsComparer.Instance);
        }
        else
            commonOptionsList = [BulkTransform.ResolveConflicts(new(), cliOptions.CommonOptions)];

        PrintLine("Writing files...", 0, cliOptions.Quiet);
        TransformAllData data = new();
        foreach (string inputPath_ in inputPaths)
        {
            string inputPath = inputPath_;
            foreach (TransformAllCommonOptions commonOptions in commonOptionsList)
            {
                // detect optimisation based on the previous transform and skip if file can be skipped altogether
                Optimisation optimisation = BulkTransform.GetOptimisation(data.InputPath, data.CommonOptions, inputPath, commonOptions);
                if (optimisation >= Optimisation.SameInputFile && data.SkipFile)
                    continue;
                data.SkipFile = false;
                optimisation = optimisation <= data.MaxOptimisation ? optimisation : data.MaxOptimisation;
                if (optimisation == Optimisation.Same)
                    continue;

                // validate the input and output files and skip to the next file if invalid
                string inputFileName = Path.GetFileName(inputPath);
                string inputFileNameNoExt = Path.GetFileNameWithoutExtension(inputPath);
                string outputFileName = commonOptions.Suffix == "/auto" ? inputFileName : inputFileNameNoExt + commonOptions.Suffix;
                string outputPath = Path.Combine(commonOptions.OutputDir, outputFileName);
                Response? response = ResponseUtils.ValidateInputOutput(ref inputPath, ref outputPath, ref data.Response, cliOptions.Response);
                if (response == null)
                    return;
                if (response == Response.No)
                    continue;
                
                // carry out transformation (with appropriate optimisation), but skip to the next file if an error occurs
                // if there is an optimisation, then skip calculating the value and instead use the previously calculated value from the data variable
                // if there isn't an optimisation, then calculate the value and put it into the data variable, so that the next file can use it if needed
                // the CSV data is sorted in such a way that rows that are optimisable are placed next to each other,
                //   thus only the file directly previous to this one is relevant
                //   (or the one before that if there was an error, but that data will still be there)
                // if in the previous transformation there was an error,
                //   then data.MaxOptimisation is set up so that the step with the error doesn't get optimised away in the next iteration
                PrintLine(outputPath, 1, cliOptions.Quiet);
                _ = Directory.CreateDirectory(commonOptions.OutputDir);
                data.CommonOptions = commonOptions;
                if (optimisation < Optimisation.SameInputFile)
                {
                    string input = File.ReadAllText(inputPath);
                    if (!BulkTransform.TryGetSortedOriginalCommentsAndJson(data, input, inputPath))
                        continue;
                }
                if (optimisation < Optimisation.SameOffset)
                    if (!BulkTransform.TryApplyOffset(data, commonOptions.TransformOptions, inputPath, outputPath))
                        continue;
                if (optimisation < Optimisation.SameFormatSameSerializationOptions)
                    if (!BulkTransform.TrySerialize(data, commonOptions.TransformOptions, inputPath, outputPath))
                        continue;
                data.MaxOptimisation = Optimisation.Same;
                File.WriteAllText(outputPath, data.Output);
            }
        }
    }
}