using System.CommandLine;

namespace TwitchChatOffset.Commands;

public class Transform
{
    public readonly Argument<string> inputArgument = Tokens.InputArgument;
    public readonly Argument<string> outputArgument = Tokens.OutputArgument;
    public readonly Option<long> startOption = Tokens.StartOption;
    public readonly Option<long> endOption = Tokens.EndOption;
    public readonly Option<Format> formatOption = Tokens.FormatOption;

    public readonly record struct Data
    (
        string InputPath,
        string OutputPath,
        long Start,
        long End,
        Format Format
    );

    public void Handle(Data data)
    {
        (string inputPath, string outputPath, long start, long end, Format format) = data;
        TransformHandler.HandleTransform(inputPath, outputPath, start, end, format);
    }
}