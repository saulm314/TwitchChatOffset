using TwitchChatOffset.ConsoleUtils;
using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
using System.CommandLine;
using System.IO;

namespace TwitchChatOffset.CommandLine;

public static class TransformAllCommand
{
    public static readonly Command Command = new("transform-all", "Transform all files in a directory whose name matches a search pattern");

    static TransformAllCommand()
    {
        Command.AddOptions<TransformAllOptions>();
        Command.SetAction(Execute);
    }

    private static void Execute(ParseResult parseResult)
    {
        TransformAllOptions options = parseResult.ParseOptions<TransformAllOptions>();
        string[] inputPaths = Directory.GetFiles(options.InputDir, options.SearchPattern);
        PrintEnumerable(inputPaths, "Input files found:", 0, options.Quiet);
        _ = Directory.CreateDirectory(options.OutputDir);
        MultiResponse? response = null;
        PrintLine("Writing files...", 0, options.Quiet);
        foreach (string inputPath in inputPaths)
        {
            string outputFileName = options.Suffix == "/auto" ? Path.GetFileName(inputPath) : Path.GetFileNameWithoutExtension(inputPath) + options.Suffix;
            string outputPath = Path.Combine(options.OutputDir, outputFileName);
            if (inputPath == outputPath && response != MultiResponse.YesToAll)
            {
                if (response == MultiResponse.NoToAll)
                    continue;
                response = ResponseUtils.GetMultiResponseInputOutputWarning(outputPath);
                if (response == MultiResponse.Cancel)
                    return;
                if (response == MultiResponse.No || response == MultiResponse.NoToAll)
                    continue;
                // response = MultiResponse.Yes or null
            }
            PrintLine(outputPath, 1, options.Quiet);
            string input = File.ReadAllText(inputPath);
            string? output = BulkTransform.TryTransform(inputPath, input, options.TransformOptions);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}