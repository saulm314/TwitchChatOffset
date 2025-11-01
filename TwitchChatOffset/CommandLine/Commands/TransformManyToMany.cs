﻿using TwitchChatOffset.CommandLine.Options;
using TwitchChatOffset.Csv;
using TwitchChatOffset.Ytt;
using System.CommandLine;
using System.IO;
using CSVFile;
using YTSubConverter.Shared;

namespace TwitchChatOffset.CommandLine.Commands;

public static class TransformManyToMany
{
    public static readonly Command Command = new("transform-many-to-many", "Transform many Json Twitch files and generate new files for each transformation");

    private static readonly Argument<string> _arguments =
        CsvArgument;

    private static readonly (Option<long>, Option<long>, Option<long>, Option<Format>, Option<AnchorPoint>, Option<long>, Option<long>, Option<double>,
        Option<Shadow>, Option<long>, Option<long>, Option<string>, Option<string>, Option<string>, Option<string>, Option<string>, Option<long>, Option<bool>)
        _options =
        (StartOption, EndOption, DelayOption, FormatOption, YttPositionOption, YttMaxMessagesOption, YttMaxCharsPerLineOption, YttScaleOption, YttShadowOption,
        YttWindowOpacityOption, YttBackgroundOpacityOption, YttTextColorOption, YttShadowColorOption, YttBackgroundColorOption, InputDirOption, OutputDirOption,
        OptionPriorityOption, QuietOption);

    static TransformManyToMany()
    {
        var a1 = _arguments;
        var (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14, o15, o16, o17, o18) = _options;
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
        Command.Add(o18);
        Command.SetAction(Execute);
    }

    private static (string, Plicit<long>, Plicit<long>, Plicit<long>, Plicit<Format>, Plicit<AnchorPoint>,
        Plicit<long>, Plicit<long>, Plicit<double>, Plicit<Shadow>, Plicit<long>, Plicit<long>, Plicit<string>,
        Plicit<string>, Plicit<string>, Plicit<string>, Plicit<string>, long, bool) GetData(ParseResult p)
    {
        var a1 = _arguments;
        var (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14, o15, o16, o17, o18) = _options;
        return (p.GetValue(a1), p.GetImplicit(o1), p.GetImplicit(o2), p.GetImplicit(o3), p.GetImplicit(o4), p.GetImplicit(o5), p.GetImplicit(o6),
            p.GetImplicit(o7), p.GetImplicit(o8), p.GetImplicit(o9), p.GetImplicit(o10), p.GetImplicit(o11), p.GetImplicit(o12), p.GetImplicit(o13),
            p.GetImplicit(o14), p.GetImplicit(o15), p.GetImplicit(o16), p.GetValue(o17), p.GetValue(o18))!;
    }

    private static void Execute(ParseResult parseResult)
    {
        var (csvPath, cliStart, cliEnd, cliDelay, cliFormat, cliYttPosition, cliYttMaxMessages, cliYttMaxCharsPerLine, cliYttScale, cliYttShadow,
            cliYttWindowOpacity, cliYttBackgroundOpacity, cliYttTextColor, cliYttShadowColor, cliYttBackgroundColor, cliInputDir, cliOutputDir,
            cliOptionPriority, quiet) = GetData(parseResult);
        using CSVReader reader = CSVReader.FromFile(csvPath, CsvUtils.CsvSettings);
        PrintLine("Writing files...", 0, quiet);
        foreach (TransformManyToManyCsvNullables nullableLine in CsvSerialization.Deserialize<TransformManyToManyCsvNullables>(reader))
        {
            TransformManyToManyCsv? line = BulkTransform.TryGetNonNullableLine(nullableLine, cliStart, cliEnd, cliDelay, cliFormat, cliYttPosition,
                cliYttMaxMessages, cliYttMaxCharsPerLine, cliYttScale, cliYttShadow, cliYttWindowOpacity, cliYttBackgroundOpacity, cliYttTextColor,
                cliYttShadowColor, cliYttBackgroundColor, cliInputDir, cliOutputDir, cliOptionPriority);
            if (line == null)
                continue;
            (string inputFile, string outputFile, long start, long end, long delay, Format format, AnchorPoint yttPosition, long yttMaxMessages,
                long yttMaxCharsPerLine, double yttScale, Shadow yttShadow, long yttWindowOpacity, long yttBackgroundOpacity, string yttTextColor,
                string yttShadowColor, string yttBackgroundColor, string inputDir, string outputDir) = line;
            PrintObjectMembers(line, outputFile, 1, quiet);
            _ = Directory.CreateDirectory(outputDir);
            string outputPath = BulkTransform.GetCombinedPath(outputDir, outputFile);
            string inputPath = BulkTransform.GetCombinedPath(inputDir, inputFile);
            string input = File.ReadAllText(inputPath);
            string? output = BulkTransform.TryTransform(inputPath, input, start, end, delay, format, yttPosition, yttMaxMessages, yttMaxCharsPerLine, yttScale,
                yttShadow, yttWindowOpacity, yttBackgroundOpacity, yttTextColor, yttShadowColor, yttBackgroundColor);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}