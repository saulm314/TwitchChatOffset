using System.CommandLine;
using System.CommandLine.Binding;

namespace TwitchChatOffset.Commands;

public class Transform : CommandBinder<Transform.Data>
{
    public override Command PCommand { get; } = new("transform", "Transform a JSON Twitch chat file and put the new contents in a new file");

    private readonly Tokens tokens = new();

    protected override void AddTokens()
    {
        PCommand.Add(tokens.InputArgument);
        PCommand.Add(tokens.OutputArgument);
        PCommand.Add(tokens.StartOption);
        PCommand.Add(tokens.EndOption);
        PCommand.Add(tokens.FormatOption);
    }

    protected override Data GetBoundValue(BindingContext bindingContext)
    {
        T Arg<T>(Argument<T> argument) => GetArgValue(argument, bindingContext);
        T Opt<T>(Option<T> option) => GetOptValue(option, bindingContext);
        return new
        (
            Arg(tokens.InputArgument),
            Arg(tokens.OutputArgument),
            Opt(tokens.StartOption),
            Opt(tokens.EndOption),
            Opt(tokens.FormatOption)
        );
    }

    public readonly record struct Data
    (
        string InputPath,
        string OutputPath,
        long Start,
        long End,
        Format Format
    );

    protected override void Handle(Data data)
    {
        (string inputPath, string outputPath, long start, long end, Format format) = data;
        TwitchChatOffset.Transform.HandleTransform(inputPath, outputPath, start, end, format);
    }
}