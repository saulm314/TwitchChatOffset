using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
using System.CommandLine;
using System.IO;
using static TwitchChatOffset.CommandLine.Arguments;

namespace TwitchChatOffset.CommandLine;

public static class TransformAllCommand
{
    public static readonly Command Command = new("transform-all", "Transform all files in a directory whose name matches a search pattern");

    static TransformAllCommand()
    {
        Command.Add(SuffixArgument);
        IOptionGroup.AddCliOptions<TransformAllOptions>(Command);
        Command.SetAction(Execute);
    }

    private static void Execute(ParseResult parseResult)
    {
        string suffix = parseResult.GetValue(SuffixArgument)!;
        TransformAllOptions options = IOptionGroup.ParseOptions<TransformAllOptions>(parseResult);
        string[] fileNames = Directory.GetFiles(options.InputDir, options.SearchPattern);
        PrintEnumerable(fileNames, "Input files found:", 0, options.Quiet);
        _ = Directory.CreateDirectory(options.OutputDir);
        PrintLine("Writing files...", 0, options.Quiet);
        foreach (string fileName in fileNames)
        {
            string outputPath = BulkTransform.GetOutputPath(fileName, options.OutputDir, suffix);
            PrintLine(outputPath, 1, options.Quiet);
            string input = File.ReadAllText(fileName);
            string? output = BulkTransform.TryTransform(fileName, input, options.TransformOptions);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}