using TwitchChatOffset.CommandLine.Options;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;

namespace TwitchChatOffset.CommandLine.Commands;

public class TransformCommand : CommandBinder<TransformCommand.Data>
{
    public override Command Command { get; } = new("transform", "Transform a JSON Twitch chat file and put the new contents in a new file");

    private readonly Tokens tokens = new();

    protected override void AddTokens()
    {
        Command.Add(tokens.InputArgument);
        Command.Add(tokens.OutputArgument);
        Command.Add(tokens.StartOption);
        Command.Add(tokens.EndOption);
        Command.Add(tokens.FormatOption);
    }

    protected override Data GetBoundValue(BindingContext b)
    {
        return new
        (
            Arg(b, tokens.InputArgument),
            Arg(b, tokens.OutputArgument),
            Opt(b, tokens.StartOption),
            Opt(b, tokens.EndOption),
            Opt(b, tokens.FormatOption)
        );
    }

    public readonly record struct Data
    (
        string InputPath,
        string OutputPath,
        OptionValueContainer<long> Start,
        OptionValueContainer<long> End,
        OptionValueContainer<Format> Format
    );

    protected override void Handle(Data data)
    {
        var (inputPath, outputPath, start, end, format) = data;
        string input = File.ReadAllText(inputPath);
        string output = Transform.DoTransform(input, start, end, format);
        File.WriteAllText(outputPath, output);
    }
}