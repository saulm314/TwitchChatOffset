using System.CommandLine;
using System.IO;

namespace TwitchChatOffset.CommandLine.Commands;

public static class TransformAll
{
    public static readonly Command Command = new("transform-all", "Transform all files in a directory whose name matches a search pattern");

    private static readonly Argument<string> _arguments =
        SuffixArgument;

    private static readonly (Option<string>, Option<string>, Option<string>, Option<Format>, Option<bool>, Option<long>, Option<long>) _options =
        (InputDirOption, SearchPatternOption, OutputDirOption, FormatOption, QuietOption, StartOption, EndOption);

    static TransformAll()
    {
        var a1 = _arguments;
        var (o1, o2, o3, o4, o5, o6, o7) = _options;
        Command.Add(a1);
        Command.Add(o1);
        Command.Add(o2);
        Command.Add(o3);
        Command.Add(o4);
        Command.Add(o5);
        Command.Add(o6);
        Command.Add(o7);
        Command.SetAction(Execute);
    }

    private static (string, string, string, string, Format, bool, long, long) GetData(ParseResult p)
    {
        var a1 = _arguments;
        var (o1, o2, o3, o4, o5, o6, o7) = _options;
        return (p.GetValue(a1), p.GetValue(o1), p.GetValue(o2), p.GetValue(o3), p.GetValue(o4), p.GetValue(o5), p.GetValue(o6), p.GetValue(o7))!;
    }

    private static void Execute(ParseResult parseResult)
    {
        var (suffix, inputDir, searchPattern, outputDir, format, quiet, start, end) = GetData(parseResult);
        string[] fileNames = Directory.GetFiles(inputDir, searchPattern);
        PrintEnumerable(fileNames, "Input files found:", 0, quiet);
        _ = Directory.CreateDirectory(outputDir);
        PrintLine("Writing files...", 0, quiet);
        foreach (string fileName in fileNames)
        {
            string outputPath = BulkTransform.GetOutputPath(fileName, outputDir, suffix);
            PrintLine(outputPath, 1, quiet);
            string input = File.ReadAllText(fileName);
            string? output = BulkTransform.TryTransform(fileName, input, start, end, format);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}