using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using CSVFile;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset.Commands;

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
        PCommand.Add(tokens.QuietOption);
    }

    protected override Data GetBoundValue(BindingContext bindingContext)
    {
        T Arg<T>(Argument<T> argument) => GetArgValue(argument, bindingContext);
        T Opt<T>(Option<T> option) => GetOptValue(option, bindingContext);
        return new
        (
            Arg(tokens.InputArgument),
            Arg(tokens.CsvArgument),
            Opt(tokens.StartOption),
            Opt(tokens.EndOption),
            Opt(tokens.FormatOption),
            Opt(tokens.OutputDirOption),
            Opt(tokens.QuietOption)
        );
    }

    public readonly record struct Data
    (
        string InputPath,
        string CsvPath,
        long Start,
        long End,
        Format PFormat,
        string OutputDir,
        bool Quiet
    );

    protected override void Handle(Data data)
    {
        (string inputPath, string csvPath, long cliStart, long cliEnd, Format cliFormat, string cliOutputDir, bool quiet) = data;
        string input = File.ReadAllText(inputPath);
        JToken parent = (JToken)JsonConvert.DeserializeObject(input)!;
        CSVReader reader = CSVReader.FromFile(csvPath, CsvUtils.csvSettings);
        PrintLine("Writing files...", 0, quiet);
        foreach (TransformOneToManyCsvNullables nullableLine in CsvSerialization.Deserialize<TransformOneToManyCsvNullables>(reader))
        {
            TransformOneToManyCsv? line = BulkTransform.TryGetNonNullableLine(nullableLine, cliStart, cliEnd, cliFormat, cliOutputDir);
            if (line == null)
                continue;
            (string outputFile, long start, long end, Format format, string outputDir) = line;
            PrintLine(outputFile, 1, quiet);
            _ = Directory.CreateDirectory(outputDir);
            string outputPath = BulkTransform.GetOutputPath(outputDir, outputFile);
            JToken parentClone = parent.DeepClone();
            string? output = BulkTransform.TryTransform(inputPath, input, start, end, format);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}