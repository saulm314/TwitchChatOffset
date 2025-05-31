using System.CommandLine;
using System.CommandLine.Binding;

namespace TwitchChatOffset.Commands;

public class Transform : BinderBase<Transform.Data>
{
    public Command Add(Command parentCommand)
    {
        Command command = new("transform", "Transform a JSON Twitch chat file and put the new contents in a new file");
        parentCommand.Add(command);
        command.SetHandler(Handle, this);
        return command;
    }

    private readonly Argument<string> inputArgument = Tokens.InputArgument;
    private readonly Argument<string> outputArgument = Tokens.OutputArgument;
    private readonly Option<long> startOption = Tokens.StartOption;
    private readonly Option<long> endOption = Tokens.EndOption;
    private readonly Option<Format> formatOption = Tokens.FormatOption;

    protected override Data GetBoundValue(BindingContext bindingContext)
        => new
        (
            bindingContext.ParseResult.GetValueForArgument(inputArgument),
            bindingContext.ParseResult.GetValueForArgument(outputArgument),
            bindingContext.ParseResult.GetValueForOption(startOption),
            bindingContext.ParseResult.GetValueForOption(endOption),
            bindingContext.ParseResult.GetValueForOption(formatOption)
        );

    public readonly record struct Data
    (
        string InputPath,
        string OutputPath,
        long Start,
        long End,
        Format Format
    );

    private void Handle(Data data)
    {
        (string inputPath, string outputPath, long start, long end, Format format) = data;
        TransformHandler.HandleTransform(inputPath, outputPath, start, end, format);
    }
}