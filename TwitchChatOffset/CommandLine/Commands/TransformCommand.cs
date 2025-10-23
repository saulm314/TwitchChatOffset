using TwitchChatOffset.Ytt;
using System.CommandLine;
using System.IO;
using YTSubConverter.Shared;

namespace TwitchChatOffset.CommandLine.Commands;

public static class TransformCommand
{
    public static readonly Command Command = new("transform", "Transform a JSON Twitch chat file and put the new contents in a new file");

    private static readonly (Argument<string>, Argument<string>) _arguments =
        (InputArgument, OutputArgument);

    private static readonly (Option<long>, Option<long>, Option<long>, Option<Format>, Option<AnchorPoint>, Option<long>, Option<long>, Option<double>,
        Option<Shadow>, Option<long>, Option<long>, Option<string>, Option<string>, Option<string>) _options =
        (StartOption, EndOption, DelayOption, FormatOption, YttPositionOption, YttMaxMessagesOption, YttMaxCharsPerLineOption, YttScaleOption, YttShadowOption,
        YttWindowOpacityOption, YttBackgroundOpacityOption, YttTextColorOption, YttShadowColorOption, YttBackgroundColorOption);

    static TransformCommand()
    {
        var (a1, a2) = _arguments;
        var (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14) = _options;
        Command.Add(a1);
        Command.Add(a2);
        Command.Add(o1);
        Command.Add(o2);
        Command.Add(o3);
        Command.Add(o4);
        Command.Add(o5);
        Command.Add(o6);
        Command.Add(o7);
        Command.Add(o8);
        Command.Add(o9);
        Command.Add(o10);
        Command.Add(o11);
        Command.Add(o12);
        Command.Add(o13);
        Command.Add(o14);
        Command.SetAction(Execute);
    }

    private static (string, string, long, long, long, Format, AnchorPoint, long, long, double, Shadow, long, long, string, string, string) GetData(ParseResult p)
    {
        var (a1, a2) = _arguments;
        var (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14) = _options;
        return (p.GetValue(a1), p.GetValue(a2), p.GetValue(o1), p.GetValue(o2), p.GetValue(o3), p.GetValue(o4), p.GetValue(o5), p.GetValue(o6),
            p.GetValue(o7), p.GetValue(o8), p.GetValue(o9), p.GetValue(o10), p.GetValue(o11), p.GetValue(o12), p.GetValue(o13), p.GetValue(o14))!;
    }

    private static void Execute(ParseResult parseResult)
    {
        var (inputPath, outputPath, start, end, delay, format, yttPosition, yttMaxMessages, yttMaxCharsPerLine, yttScale, yttShadow, yttWindowOpacity,
            yttBackgroundOpacity, yttTextColor, yttShadowColor, yttBackgroundColor) = GetData(parseResult);
        string input = File.ReadAllText(inputPath);
        string output = Transform.DoTransform(input, start, end, delay, format, yttPosition, yttMaxMessages, yttMaxCharsPerLine, yttScale, yttShadow,
            yttWindowOpacity, yttBackgroundOpacity, yttTextColor, yttShadowColor, yttBackgroundColor);
        File.WriteAllText(outputPath, output);
    }
}