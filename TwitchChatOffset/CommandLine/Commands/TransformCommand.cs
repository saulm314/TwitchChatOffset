using System.CommandLine;
using System.IO;
using YTSubConverter.Shared;

namespace TwitchChatOffset.CommandLine.Commands;

public static class TransformCommand
{
    public static readonly Command Command = new("transform", "Transform a JSON Twitch chat file and put the new contents in a new file");

    private static readonly (Argument<string>, Argument<string>) _arguments =
        (InputArgument, OutputArgument);

    private static readonly (Option<long>, Option<long>, Option<long>, Option<Format>, Option<AnchorPoint>, Option<long>) _options =
        (StartOption, EndOption, DelayOption, FormatOption, YttPositionOption, YttMaxMessagesOption);

    static TransformCommand()
    {
        var (a1, a2) = _arguments;
        var (o1, o2, o3, o4, o5, o6) = _options;
        Command.Add(a1);
        Command.Add(a2);
        Command.Add(o1);
        Command.Add(o2);
        Command.Add(o3);
        Command.Add(o4);
        Command.Add(o5);
        Command.Add(o6);
        Command.SetAction(Execute);
    }

    private static (string, string, long, long, long, Format, AnchorPoint, long) GetData(ParseResult p)
    {
        var (a1, a2) = _arguments;
        var (o1, o2, o3, o4, o5, o6) = _options;
        return (p.GetValue(a1), p.GetValue(a2), p.GetValue(o1), p.GetValue(o2), p.GetValue(o3), p.GetValue(o4), p.GetValue(o5), p.GetValue(o6))!;
    }

    private static void Execute(ParseResult parseResult)
    {
        var (inputPath, outputPath, start, end, delay, format, yttPosition, yttMaxMessages) = GetData(parseResult);
        string input = File.ReadAllText(inputPath);
        string output = Transform.DoTransform(input, start, end, delay, format, yttPosition, yttMaxMessages);
        File.WriteAllText(outputPath, output);
    }
}