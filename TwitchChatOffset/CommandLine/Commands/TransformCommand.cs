using System.CommandLine;
using System.IO;

namespace TwitchChatOffset.CommandLine.Commands;

public static class TransformCommand
{
    public static readonly Command Command = new("transform", "Transform a JSON Twitch chat file and put the new contents in a new file");

    private static readonly (Argument<string>, Argument<string>) _arguments =
        (InputArgument, OutputArgument);

    private static readonly (Option<long>, Option<long>, Option<Format>) _options =
        (StartOption, EndOption, FormatOption);

    static TransformCommand()
    {
        var (a1, a2) = _arguments;
        var (o1, o2, o3) = _options;
        Command.Add(a1);
        Command.Add(a2);
        Command.Add(o1);
        Command.Add(o2);
        Command.Add(o3);
        Command.SetAction(Execute);
    }

    private static (string, string, long, long, Format) GetData(ParseResult p)
    {
        var (a1, a2) = _arguments;
        var (o1, o2, o3) = _options;
        return (p.GetValue(a1), p.GetValue(a2), p.GetValue(o1), p.GetValue(o2), p.GetValue(o3))!;
    }

    private static void Execute(ParseResult parseResult)
    {
        var (inputPath, outputPath, start, end, format) = GetData(parseResult);
        string input = File.ReadAllText(inputPath);
        string output = Transform.DoTransform(input, start, end, format);
        File.WriteAllText(outputPath, output);
    }
}