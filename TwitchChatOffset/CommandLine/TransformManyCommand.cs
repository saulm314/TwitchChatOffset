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
            Optimisation optimisation = BulkTransform.GetOptimisation(data.CommonOptions, commonOptions);
            if (optimisation >= Optimisation.SameInputFile && data.SkipFile)
                continue;
            data.SkipFile = false;
            optimisation = optimisation <= data.MaxOptimisation ? optimisation : data.MaxOptimisation;
            if (optimisation == Optimisation.Same)
                continue;
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
            if (optimisation < Optimisation.SameFormatSameSubtitleOptions)
                if (!BulkTransform.TrySerialize(data, commonOptions.TransformOptions, inputPath, outputPath))
                    continue;
            data.MaxOptimisation = Optimisation.Same;
            File.WriteAllText(outputPath, data.Output);
        }
    }
}