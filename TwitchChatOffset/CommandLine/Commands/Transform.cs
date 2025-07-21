using TwitchChatOffset.CommandLine.Arguments;
using TwitchChatOffset.CommandLine.Options;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using TransformHandler = TwitchChatOffset.Transform;

namespace TwitchChatOffset.CommandLine.Commands;

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
        T Arg<T>(TCOArgumentBase<T> argument) => argument.GetValue(bindingContext);
        T Opt<T>(TCOOptionBase<T> option) => option.GetValue(bindingContext);
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
        string input = File.ReadAllText(inputPath);
        string output = TransformHandler.MTransform(input, start, end, format);
        File.WriteAllText(outputPath, output);
    }
}