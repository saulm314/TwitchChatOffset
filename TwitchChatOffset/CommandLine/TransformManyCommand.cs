using TwitchChatOffset.ConsoleUtils;
using TwitchChatOffset.Csv;
using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
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
        using CSVReader reader = CSVReader.FromFile(csvPath, CsvUtils.CsvSettings);
        MultiResponse? globalResponse = null;
        PrintLine("Writing files...", 0, cliOptions.Quiet);
        foreach (TransformManyCsvOptions csvOptions in CsvSerialization.Deserialize<TransformManyCsvOptions>(reader))
        {
            TransformManyCommonOptions commonOptions = BulkTransform.ResolveConflicts(csvOptions.CommonOptions, cliOptions.CommonOptions);
            if (!IOUtils.ValidateInputFileNameNotEmpty(commonOptions.InputFile) || !IOUtils.ValidateOutputFileNameNotEmpty(commonOptions.OutputFile))
                continue;
            string outputFileName =
                commonOptions.Suffix == "/auto" ?
                commonOptions.OutputFile :
                Path.GetFileNameWithoutExtension(commonOptions.OutputFile) + commonOptions.Suffix;
            string inputPath = Path.Combine(commonOptions.InputDir, commonOptions.InputFile);
            string outputPath = Path.Combine(commonOptions.OutputDir, outputFileName);
            Response? response = ResponseUtils.ValidateInputOutput(ref inputPath, ref outputPath, ref globalResponse, cliOptions.Response);
            if (response == null)
                return;
            if (response == Response.No)
                continue;
            if (!File.ValidateFileExists(inputPath))
                continue;
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