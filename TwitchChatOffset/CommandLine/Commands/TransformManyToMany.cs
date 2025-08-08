using TwitchChatOffset.CommandLine.Options;
using TwitchChatOffset.Csv;
using System.CommandLine;
using System.IO;
using CSVFile;

namespace TwitchChatOffset.CommandLine.Commands;

public static class TransformManyToMany
{
    public static readonly Command Command = new("transform-many-to-many", "Transform many Json Twitch files and generate new files for each transformation");

    private static readonly Argument<string> _arguments =
        CsvArgument;

    private static readonly (Option<long>, Option<long>, Option<Format>, Option<string>, Option<long>, Option<bool>) _options =
        (StartOption, EndOption, FormatOption, OutputDirOption, OptionPriorityOption, QuietOption);

    static TransformManyToMany()
    {
        var a1 = _arguments;
        var (o1, o2, o3, o4, o5, o6) = _options;
        Command.Add(a1);
        Command.Add(o1);
        Command.Add(o2);
        Command.Add(o3);
        Command.Add(o4);
        Command.Add(o5);
        Command.Add(o6);
        Command.SetAction(Execute);
    }

    private static (string, ImplicitValue<long>, ImplicitValue<long>, ImplicitValue<Format>, ImplicitValue<string>, long, bool) GetData(ParseResult p)
    {
        var a1 = _arguments;
        var (o1, o2, o3, o4, o5, o6) = _options;
        return (p.GetValue(a1), p.GetImplicit(o1), p.GetImplicit(o2), p.GetImplicit(o3), p.GetImplicit(o4), p.GetValue(o5), p.GetValue(o6))!;
    }

    private static void Execute(ParseResult parseResult)
    {
        var (csvPath, cliStart, cliEnd, cliFormat, cliOutputDir, cliOptionPriority, quiet) = GetData(parseResult);
        using CSVReader reader = CSVReader.FromFile(csvPath, CsvUtils.CsvSettings);
        PrintLine("Writing files...", 0, quiet);
        foreach (TransformManyToManyCsvNullables nullableLine in CsvSerialization.Deserialize<TransformManyToManyCsvNullables>(reader))
        {
            TransformManyToManyCsv? line = BulkTransform.TryGetNonNullableLine(nullableLine, cliStart, cliEnd, cliFormat, cliOutputDir, cliOptionPriority);
            if (line == null)
                continue;
            (string inputFile, string outputFile, long start, long end, Format format, string outputDir) = line;
            PrintObjectMembers(line, outputFile, 1, quiet);
            _ = Directory.CreateDirectory(outputDir);
            string outputPath = BulkTransform.GetOutputPath(outputDir, outputFile);
            string input = File.ReadAllText(inputFile);
            string? output = BulkTransform.TryTransform(inputFile, input, start, end, format);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}