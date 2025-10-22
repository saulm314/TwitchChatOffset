using System.CommandLine;
using System.IO;
using TwitchChatOffset.Ytt;
using YTSubConverter.Shared;

namespace TwitchChatOffset.CommandLine.Commands;

public static class TransformAll
{
    public static readonly Command Command = new("transform-all", "Transform all files in a directory whose name matches a search pattern");

    private static readonly Argument<string> _arguments =
        SuffixArgument;

    private static readonly (Option<string>, Option<string>, Option<string>, Option<Format>, Option<AnchorPoint>, Option<long>, Option<long>, Option<double>,
        Option<Shadow>, Option<long>, Option<string>, Option<string>, Option<string>, Option<bool>, Option<long>, Option<long>, Option<long>)
        _options = (InputDirOption, SearchPatternOption, OutputDirOption, FormatOption, YttPositionOption, YttMaxMessagesOption, YttMaxCharsPerLineOption,
        YttScaleOption, YttShadowOption, YttBackgroundOpacityOption, YttTextColorOption, YttShadowColorOption, YttBackgroundColorOption, QuietOption,
        StartOption, EndOption, DelayOption);

    static TransformAll()
    {
        var a1 = _arguments;
        var (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14, o15, o16, o17) = _options;
        Command.Add(a1);
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
        Command.Add(o15);
        Command.Add(o16);
        Command.Add(o17);
        Command.SetAction(Execute);
    }

    private static (string, string, string, string, Format, AnchorPoint, long, long, double, Shadow, long, string, string, string, bool, long, long, long)
        GetData(ParseResult p)
    {
        var a1 = _arguments;
        var (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14, o15, o16, o17) = _options;
        return (p.GetValue(a1), p.GetValue(o1), p.GetValue(o2), p.GetValue(o3), p.GetValue(o4), p.GetValue(o5), p.GetValue(o6), p.GetValue(o7), p.GetValue(o8),
            p.GetValue(o9), p.GetValue(o10), p.GetValue(o11), p.GetValue(o12), p.GetValue(o13), p.GetValue(o14), p.GetValue(o15), p.GetValue(o16),
            p.GetValue(o17))!;
    }

    private static void Execute(ParseResult parseResult)
    {
        var (suffix, inputDir, searchPattern, outputDir, format, yttPosition, yttMaxMessages, yttMaxCharsPerLine, yttScale, yttShadow, yttBackgroundOpacity,
            yttTextColor, yttShadowColor, yttBackgroundColor, quiet, start, end, delay) = GetData(parseResult);
        string[] fileNames = Directory.GetFiles(inputDir, searchPattern);
        PrintEnumerable(fileNames, "Input files found:", 0, quiet);
        _ = Directory.CreateDirectory(outputDir);
        PrintLine("Writing files...", 0, quiet);
        foreach (string fileName in fileNames)
        {
            string outputPath = BulkTransform.GetOutputPath(fileName, outputDir, suffix);
            PrintLine(outputPath, 1, quiet);
            string input = File.ReadAllText(fileName);
            string? output = BulkTransform.TryTransform(fileName, input, start, end, delay, format, yttPosition, yttMaxMessages, yttMaxCharsPerLine, yttScale,
                yttShadow, yttBackgroundOpacity, yttTextColor, yttShadowColor, yttBackgroundColor);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}