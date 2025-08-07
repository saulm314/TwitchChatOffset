using TwitchChatOffset.CommandLine.Options;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;

namespace TwitchChatOffset.CommandLine.Commands;

public class TransformAll : CommandBinder<TransformAll.Data>
{
    public override Command Command { get; } = new("transform-all", "Transform all files in a directory whose name matches a search pattern");

    private readonly Tokens tokens = new();

    protected override void AddTokens()
    {
        Command.Add(tokens.SuffixArgument);
        Command.Add(tokens.InputDirOption);
        Command.Add(tokens.SearchPatternOption);
        Command.Add(tokens.OutputDirOption);
        Command.Add(tokens.FormatOption);
        Command.Add(tokens.QuietOption);
        Command.Add(tokens.StartOption);
        Command.Add(tokens.EndOption);
    }

    protected override Data GetBoundValue(BindingContext b)
    {
        return new
        (
            Arg(b, tokens.SuffixArgument),
            Opt(b, tokens.InputDirOption),
            Opt(b, tokens.SearchPatternOption),
            Opt(b, tokens.OutputDirOption),
            Opt(b, tokens.FormatOption),
            Opt(b, tokens.QuietOption),
            Opt(b, tokens.StartOption),
            Opt(b, tokens.EndOption)
        );
    }

    public readonly record struct Data
    (
        string Suffix,
        OptionValueContainer<string> InputDir,
        OptionValueContainer<string> SearchPattern,
        OptionValueContainer<string> OutputDir,
        OptionValueContainer<Format> Format,
        OptionValueContainer<bool> Quiet,
        OptionValueContainer<long> Start,
        OptionValueContainer<long> End
    );

    protected override void Handle(Data data)
    {
        var (suffix, inputDir, searchPattern, outputDir, format, quiet, start, end) = data;
        string[] fileNames = Directory.GetFiles(inputDir, searchPattern);
        PrintEnumerable(fileNames, "Input files found:", 0, quiet);
        _ = Directory.CreateDirectory(outputDir);
        PrintLine("Writing files...", 0, quiet);
        foreach (string fileName in fileNames)
        {
            string outputPath = BulkTransform.GetOutputPath(fileName, outputDir, suffix);
            PrintLine(outputPath, 1, quiet);
            string input = File.ReadAllText(fileName);
            string? output = BulkTransform.TryTransform(fileName, input, start, end, format);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}