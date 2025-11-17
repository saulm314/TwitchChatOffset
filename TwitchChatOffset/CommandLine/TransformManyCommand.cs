using TwitchChatOffset.ConsoleUtils;
using TwitchChatOffset.Csv;
using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Optimisations;
using TwitchChatOffset.Options.Groups;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using CSVFile;
using static TwitchChatOffset.CommandLine.Arguments;

namespace TwitchChatOffset.CommandLine;

public static class TransformManyCommand
{
    public static readonly Command Command = new("transform-many", "Perform many JSON Twitch chat file transformations and generate new files");

    static TransformManyCommand()
    {
        Command.Add(CsvArgument);
        Command.AddOptions<TransformManyCliOptions>();
        Command.SetAction(Execute);
    }

    private static void Execute(ParseResult parseResult)
    {
        string csvPath = parseResult.GetValue(CsvArgument)!;
        TransformManyCliOptions cliOptions = parseResult.ParseOptions<TransformManyCliOptions>();

        PrintLine("Reading CSV data...", 0, cliOptions.Quiet);
        using CSVReader reader = CSVReader.FromFile(csvPath, CsvUtils.CsvSettings);
        List<TransformManyCommonOptions> commonOptionsList = [];
        foreach (TransformManyCommonOptions commonOptions in CsvSerialization.Deserialize<TransformManyCommonOptions>(reader))
            commonOptionsList.Add(BulkTransform.ResolveConflicts(commonOptions, cliOptions.CommonOptions));

        PrintLine("Sorting CSV data...", 0, cliOptions.Quiet);
        commonOptionsList.Sort(TransformManyCommonOptionsComparer.Instance);

        PrintLine("Writing files...", 0, cliOptions.Quiet);
        TransformManyData data = new();
        foreach (TransformManyCommonOptions commonOptions in commonOptionsList)
        {
            // detect optimisation based on the previous transform and skip if file can be skipped altogether
            Optimisation optimisation = BulkTransform.GetOptimisation(data.CommonOptions, commonOptions);
            if (optimisation >= Optimisation.SameInputFile && data.SkipFile)
                continue;
            data.SkipFile = false;
            optimisation = optimisation <= data.MaxOptimisation ? optimisation : data.MaxOptimisation;
            if (optimisation == Optimisation.Same)
                continue;

            // validate the input and output files and skip to the next file if invalid
            if (!IOUtils.ValidateInputFileNameNotEmpty(commonOptions.InputFile) || !IOUtils.ValidateOutputFileNameNotEmpty(commonOptions.OutputFile))
                continue;
            string outputFileName =
                commonOptions.Suffix == "/auto" ?
                commonOptions.OutputFile :
                Path.GetFileNameWithoutExtension(commonOptions.OutputFile) + commonOptions.Suffix;
            string inputPath = Path.Combine(commonOptions.InputDir, commonOptions.InputFile);
            string outputPath = Path.Combine(commonOptions.OutputDir, outputFileName);
            Response? response = ResponseUtils.ValidateInputOutput(ref inputPath, ref outputPath, ref data.Response, cliOptions.Response);
            if (response == null)
                return;
            if (response == Response.No)
                continue;
            if (!File.ValidateFileExists(inputPath))
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