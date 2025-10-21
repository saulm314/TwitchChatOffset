using TwitchChatOffset.CommandLine.Options;
using TwitchChatOffset.Csv;
using TwitchChatOffset.Json;
using System.CommandLine;
using System.IO;
using CSVFile;
using Newtonsoft.Json.Linq;
using YTSubConverter.Shared;

namespace TwitchChatOffset.CommandLine.Commands;

public static class TransformOneToMany
{
    public static readonly Command Command = new("transform-one-to-many", "Transform a JSON Twitch file into many new files");

    private static readonly (Argument<string>, Argument<string>) _arguments =
        (InputArgument, CsvArgument);

    private static readonly (Option<long>, Option<long>, Option<Format>, Option<AnchorPoint>, Option<string>, Option<long>, Option<bool>) _options
        = (StartOption, EndOption, FormatOption, YttPositionOption, OutputDirOption, OptionPriorityOption, QuietOption);

    static TransformOneToMany()
    {
        var (a1, a2) = _arguments;
        var (o1, o2, o3, o4, o5, o6, o7) = _options;
        Command.Add(a1);
        Command.Add(o1);
        Command.Add(o2);
        Command.Add(o3);
        Command.Add(o4);
        Command.Add(o5);
        Command.Add(o6);
        Command.Add(o7);
        Command.SetAction(Execute);
    }

    private static (string, string, ImplicitValue<long>, ImplicitValue<long>, ImplicitValue<Format>, ImplicitValue<AnchorPoint>, ImplicitValue<string>, long,
        bool) GetData(ParseResult p)
    {
        var (a1, a2) = _arguments;
        var (o1, o2, o3, o4, o5, o6, o7) = _options;
        return (p.GetValue(a1), p.GetValue(a2), p.GetImplicit(o1), p.GetImplicit(o2), p.GetImplicit(o3), p.GetImplicit(o4), p.GetImplicit(o5), p.GetValue(o6),
            p.GetValue(o7))!;
    }

    private static void Execute(ParseResult parseResult)
    {
        var (inputPath, csvPath, cliStart, cliEnd, cliFormat, cliYttPosition, cliOutputDir, cliOptionPriority, quiet) = GetData(parseResult);
        string input = File.ReadAllText(inputPath);
        JToken json = JsonUtils.Deserialize(input);
        using CSVReader reader = CSVReader.FromFile(csvPath, CsvUtils.CsvSettings);
        PrintLine("Writing files...", 0, quiet);
        foreach (TransformOneToManyCsvNullables nullableLine in CsvSerialization.Deserialize<TransformOneToManyCsvNullables>(reader))
        {
            TransformOneToManyCsv? line =
                BulkTransform.TryGetNonNullableLine(nullableLine, cliStart, cliEnd, cliFormat, cliYttPosition, cliOutputDir, cliOptionPriority);
            if (line == null)
                continue;
            (string outputFile, long start, long end, Format format, AnchorPoint yttPosition, string outputDir) = line;
            PrintObjectMembers(line, outputFile, 1, quiet);
            _ = Directory.CreateDirectory(outputDir);
            string outputPath = BulkTransform.GetOutputPath(outputDir, outputFile);
            string? output = BulkTransform.TryTransform(inputPath, json, start, end, format, yttPosition);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}