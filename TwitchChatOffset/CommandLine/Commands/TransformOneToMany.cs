using TwitchChatOffset.CommandLine.Options;
using TwitchChatOffset.Csv;
using TwitchChatOffset.Json;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using CSVFile;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset.CommandLine.Commands;

public class TransformOneToMany : CommandBinder<TransformOneToMany.Data>
{
    public override Command Command { get; } = new("transform-one-to-many", "Transform a JSON Twitch file into many new files");

    private readonly Tokens tokens = new();

    protected override void AddTokens()
    {
        Command.Add(tokens.InputArgument);
        Command.Add(tokens.CsvArgument);
        Command.Add(tokens.EndOption);
        Command.Add(tokens.StartOption);
        Command.Add(tokens.FormatOption);
        Command.Add(tokens.OutputDirOption);
        Command.Add(tokens.OptionPriorityOption);
        Command.Add(tokens.QuietOption);
    }

    protected override Data GetBoundValue(BindingContext b)
    {
        return new
        (
            Arg(b, tokens.InputArgument),
            Arg(b, tokens.CsvArgument),
            Opt(b, tokens.StartOption),
            Opt(b, tokens.EndOption),
            Opt(b, tokens.FormatOption),
            Opt(b, tokens.OutputDirOption),
            Opt(b, tokens.OptionPriorityOption),
            Opt(b, tokens.QuietOption)
        );
    }

    public readonly record struct Data
    (
        string InputPath,
        string CsvPath,
        OptionValueContainer<long> Start,
        OptionValueContainer<long> End,
        OptionValueContainer<Format> PFormat,
        OptionValueContainer<string> OutputDir,
        OptionValueContainer<long> OptionPriority,
        OptionValueContainer<bool> Quiet
    );

    protected override void Handle(Data data)
    {
        var (inputPath, csvPath, cliStart, cliEnd, cliFormat, cliOutputDir, cliOptionPriority, quiet) = data;
        string input = File.ReadAllText(inputPath);
        JToken parent = JsonUtils.Deserialize(input);
        using CSVReader reader = CSVReader.FromFile(csvPath, CsvUtils.csvSettings);
        PrintLine("Writing files...", 0, quiet);
        foreach (TransformOneToManyCsvNullables nullableLine in CsvSerialization.Deserialize<TransformOneToManyCsvNullables>(reader))
        {
            TransformOneToManyCsv? line = BulkTransform.TryGetNonNullableLine(nullableLine, cliStart, cliEnd, cliFormat, cliOutputDir, cliOptionPriority);
            if (line == null)
                continue;
            (string outputFile, long start, long end, Format format, string outputDir) = line;
            PrintObjectMembers(line, outputFile, 1, quiet);
            _ = Directory.CreateDirectory(outputDir);
            string outputPath = BulkTransform.GetOutputPath(outputDir, outputFile);
            string? output = BulkTransform.TryTransform(inputPath, parent, start, end, format);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}