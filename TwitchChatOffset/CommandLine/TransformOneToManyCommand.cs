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
        IOptionGroup.AddCliOptions<TransformOneToManyCliOptions>(Command);
        Command.SetAction(Execute);
    }

    private static void Execute(ParseResult parseResult)
    {
        string inputPath = parseResult.GetValue(InputArgument)!;
        string csvPath = parseResult.GetValue(CsvArgument)!;
        TransformOneToManyCliOptions cliOptions = IOptionGroup.ParseOptions<TransformOneToManyCliOptions>(parseResult);
        string input = File.ReadAllText(inputPath);
        JToken json = JsonUtils.Deserialize(input);
        using CSVReader reader = CSVReader.FromFile(csvPath, CsvUtils.CsvSettings);
        PrintLine("Writing files...", 0, cliOptions.Quiet);
        foreach (TransformOneToManyCsvOptions csvOptions in CsvSerialization.Deserialize<TransformOneToManyCsvOptions>(reader))
        {
            if (!csvOptions.OutputFile.Explicit)
            {
                PrintError("Output file must not be empty! Skipping...", 1);
                continue;
            }
            TransformOneToManyCommonOptions commonOptions = BulkTransform.ResolveConflicts(csvOptions.CommonOptions, cliOptions.CommonOptions);
            PrintLine(csvOptions.OutputFile, 1, cliOptions.Quiet);
            _ = Directory.CreateDirectory(commonOptions.OutputDir);
            string outputPath = BulkTransform.GetCombinedPath(commonOptions.OutputDir, csvOptions.OutputFile);
            string? output = BulkTransform.TryTransform(inputPath, json, commonOptions.TransformOptions);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}