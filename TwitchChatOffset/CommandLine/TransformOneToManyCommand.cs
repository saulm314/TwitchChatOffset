using TwitchChatOffset.ConsoleUtils;
using TwitchChatOffset.Csv;
using TwitchChatOffset.Json;
using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
using System.CommandLine;
using System.IO;
using CSVFile;
using Newtonsoft.Json.Linq;
using static TwitchChatOffset.CommandLine.Arguments;

namespace TwitchChatOffset.CommandLine;

public static class TransformOneToManyCommand
{
    public static readonly Command Command = new("transform-one-to-many", "Transform a JSON Twitch file into many new files");

    static TransformOneToManyCommand()
    {
        Command.Add(InputArgument);
        Command.Add(CsvArgument);
        Command.AddOptions<TransformOneToManyCliOptions>();
        Command.SetAction(Execute);
    }

    private static void Execute(ParseResult parseResult)
    {
        string inputPath = parseResult.GetValue(InputArgument)!;
        string csvPath = parseResult.GetValue(CsvArgument)!;
        TransformOneToManyCliOptions cliOptions = parseResult.ParseOptions<TransformOneToManyCliOptions>();
        string input = File.ReadAllText(inputPath);
        JToken json = JsonUtils.Deserialize(input);
        using CSVReader reader = CSVReader.FromFile(csvPath, CsvUtils.CsvSettings);
        MultiResponse? globalResponse = null;
        PrintLine("Writing files...", 0, cliOptions.Quiet);
        foreach (TransformOneToManyCsvOptions csvOptions in CsvSerialization.Deserialize<TransformOneToManyCsvOptions>(reader))
        {
            TransformOneToManyCommonOptions commonOptions = BulkTransform.ResolveConflicts(csvOptions.CommonOptions, cliOptions.CommonOptions);
            if (!IOUtils.ValidateOutputFileNameNotEmpty(commonOptions.OutputFile))
                continue;
            string outputFileName =
                commonOptions.Suffix == "/auto" ?
                commonOptions.OutputFile :
                Path.GetFileNameWithoutExtension(commonOptions.OutputFile) + commonOptions.Suffix;
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
            string? output = BulkTransform.TryTransform(inputPath, json, commonOptions.TransformOptions);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}