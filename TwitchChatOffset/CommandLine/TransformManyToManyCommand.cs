using TwitchChatOffset.ConsoleUtils;
using TwitchChatOffset.Csv;
using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
using System.CommandLine;
using System.IO;
using CSVFile;
using static TwitchChatOffset.CommandLine.Arguments;

namespace TwitchChatOffset.CommandLine;

public static class TransformManyToManyCommand
{
    public static readonly Command Command = new("transform-many-to-many", "Transform many Json Twitch files and generate new files for each transformation");

    static TransformManyToManyCommand()
    {
        Command.Add(CsvArgument);
        Command.AddOptions<TransformManyToManyCliOptions>();
        Command.SetAction(Execute);
    }

    private static void Execute(ParseResult parseResult)
    {
        string csvPath = parseResult.GetValue(CsvArgument)!;
        TransformManyToManyCliOptions cliOptions = parseResult.ParseOptions<TransformManyToManyCliOptions>();
        using CSVReader reader = CSVReader.FromFile(csvPath, CsvUtils.CsvSettings);
        MultiResponse? response = null;
        PrintLine("Writing files...", 0, cliOptions.Quiet);
        foreach (TransformManyToManyCsvOptions csvOptions in CsvSerialization.Deserialize<TransformManyToManyCsvOptions>(reader))
        {
            if (!csvOptions.InputFile.Explicit)
            {
                PrintError("Input file must not be empty! Skipping...", 1);
                continue;
            }
            if (!csvOptions.OutputFile.Explicit)
            {
                PrintError("Output file must not be empty! Skipping...", 1);
                continue;
            }
            TransformManyToManyCommonOptions commonOptions = BulkTransform.ResolveConflicts(csvOptions.CommonOptions, cliOptions.CommonOptions);
            string outputFileName =
                commonOptions.Suffix == "/auto" ?
                csvOptions.OutputFile :
                Path.GetFileNameWithoutExtension(csvOptions.OutputFile) + commonOptions.Suffix;
            string inputPath = Path.Combine(commonOptions.InputDir, csvOptions.InputFile);
            string outputPath = Path.Combine(commonOptions.OutputDir, outputFileName);
            if (inputPath == outputPath && response != MultiResponse.YesToAll)
            {
                if (response == MultiResponse.NoToAll)
                    continue;
                response = ResponseUtils.GetMultiResponseInputOutputWarning(outputPath);
                if (response == MultiResponse.Cancel)
                    return;
                if (response == MultiResponse.No || response == MultiResponse.NoToAll)
                    continue;
                // response = MultiResponse.Yes or null
            }
            PrintLine(outputPath, 1, cliOptions.Quiet);
            _ = Directory.CreateDirectory(commonOptions.OutputDir);
            string input = File.ReadAllText(inputPath);
            string? output = BulkTransform.TryTransform(inputPath, input, commonOptions.TransformOptions);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}