using TwitchChatOffset.CommandLine.Options;
using TwitchChatOffset.Csv;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using CSVFile;

namespace TwitchChatOffset.CommandLine.Commands;

public class TransformManyToMany : CommandBinder<TransformManyToMany.Data>
{
    public override Command Command { get; } = new("transform-many-to-many", "Transform many Json Twitch files and generate new files for each transformation");

    private readonly Tokens tokens = new();

    protected override void AddTokens()
    {
        Command.Add(tokens.CsvArgument);
        Command.Add(tokens.StartOption);
        Command.Add(tokens.EndOption);
        Command.Add(tokens.FormatOption);
        Command.Add(tokens.OutputDirOption);
        Command.Add(tokens.OptionPriorityOption);
        Command.Add(tokens.QuietOption);
    }

    protected override Data GetBoundValue(BindingContext b)
    {
        return new
        (
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
        string CsvPath,
        OptionValueContainer<long> Start,
        OptionValueContainer<long> End,
        OptionValueContainer<Format> Format,
        OptionValueContainer<string> OutputDir,
        OptionValueContainer<long> OptionPriority,
        OptionValueContainer<bool> Quiet
    );

    protected override void Handle(Data data)
    {
        var (csvPath, cliStart, cliEnd, cliFormat, cliOutputDir, cliOptionPriority, quiet) = data;
        using CSVReader reader = CSVReader.FromFile(csvPath, CsvUtils.csvSettings);
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