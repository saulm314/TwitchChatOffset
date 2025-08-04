using TwitchChatOffset.CommandLine.Arguments;
using TwitchChatOffset.CommandLine.Options;
using TwitchChatOffset.Csv;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using CSVFile;

namespace TwitchChatOffset.CommandLine.Commands;

public class TransformManyToMany : CommandBinder<TransformManyToMany.Data>
{
    public override Command PCommand { get; } = new("transform-many-to-many", "Transform many Json Twitch files and generate new files for each transformation");

    private readonly Tokens tokens = new();

    protected override void AddTokens()
    {
        PCommand.Add(tokens.CsvArgument);
        PCommand.Add(tokens.StartOption);
        PCommand.Add(tokens.EndOption);
        PCommand.Add(tokens.FormatOption);
        PCommand.Add(tokens.OutputDirOption);
        PCommand.Add(tokens.OptionPriorityOption);
        PCommand.Add(tokens.QuietOption);
    }

    protected override Data GetBoundValue(BindingContext bindingContext)
    {
        T Arg<T>(TCOArgumentBase<T> argument) => argument.GetValue(bindingContext);
        T Opt<T>(TCOOptionBase<T> option) => option.GetValue(bindingContext);

        NullableOption<T> NullOpt<T>(NullableOption<T> option) where T : notnull
        {
            _ = Opt(option);
            return option;
        }

        return new
        (
            Arg(tokens.CsvArgument),
            NullOpt(tokens.StartOption),
            NullOpt(tokens.EndOption),
            NullOpt(tokens.FormatOption),
            NullOpt(tokens.OutputDirOption),
            Opt(tokens.OptionPriorityOption),
            Opt(tokens.QuietOption)
        );
    }

    public readonly record struct Data
    (
        string CsvPath,
        NullableOption<long> Start,
        NullableOption<long> End,
        NullableOption<Format> PFormat,
        NullableOption<string> OutputDir,
        long OptionPriority,
        bool Quiet
    );

    protected override void Handle(Data data)
    {
        (string csvPath, NullableOption<long> cliStart, NullableOption<long> cliEnd, NullableOption<Format> cliFormat, NullableOption<string> cliOutputDir,
            long cliOptionPriority, bool quiet) = data;
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