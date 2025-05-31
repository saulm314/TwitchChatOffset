using System.CommandLine;
using System.CommandLine.Binding;
using static TwitchChatOffset.Tokens;

namespace TwitchChatOffset.Commands;

public class Transform : CommandBinder<Transform.Data>
{
    public override Command PCommand { get; } = new("transform", "Transform a JSON Twitch chat file and put the new contents in a new file");

    protected override Data GetBoundValue(BindingContext bindingContext)
    {
        T Arg<T>(Argument<T> argument) => GetArgValue(argument, bindingContext);
        T Opt<T>(Option<T> option) => GetOptValue(option, bindingContext);
        return new
        (
            Arg(InputArgument),
            Arg(OutputArgument),
            Opt(StartOption),
            Opt(EndOption),
            Opt(FormatOption)
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
        TransformHandler.HandleTransform(inputPath, outputPath, start, end, format);
    }
}