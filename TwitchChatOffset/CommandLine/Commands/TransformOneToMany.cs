using TwitchChatOffset.CommandLine.Arguments;
using TwitchChatOffset.CommandLine.Options;
using TwitchChatOffset.Csv;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using CSVFile;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset.CommandLine.Commands;

public class TransformOneToMany : CommandBinder<TransformOneToMany.Data>
{
    public override Command PCommand { get; } = new("transform-one-to-many", "Transform a JSON Twitch file into many new files");

    private readonly Tokens tokens = new();

    protected override void AddTokens()
    {
        PCommand.Add(tokens.InputArgument);
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
            Arg(tokens.InputArgument),
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
        string InputPath,
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
        (string inputPath, string csvPath, NullableOption<long> cliStart, NullableOption<long> cliEnd, NullableOption<Format> cliFormat,
            NullableOption<string> cliOutputDir, long cliOptionPriority, bool quiet) = data;
        string input = File.ReadAllText(inputPath);
        JToken parent = (JToken)JsonConvert.DeserializeObject(input)!;
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