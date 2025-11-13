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
        string[] fileNames = Directory.GetFiles(options.InputDir, options.SearchPattern);
        PrintEnumerable(fileNames, "Input files found:", 0, options.Quiet);
        _ = Directory.CreateDirectory(options.OutputDir);
        MultiResponse? response = null;
        PrintLine("Writing files...", 0, options.Quiet);
        foreach (string fileName in fileNames)
        {
            string inputPath = BulkTransform.GetCombinedPath(options.InputDir, fileName);
            string outputPath = BulkTransform.GetOutputPath(fileName, options.OutputDir, options.Suffix);
            if (inputPath == outputPath && response != MultiResponse.YesToAll)
            {
                if (response == MultiResponse.NoToAll)
                    continue;
                response = ResponseUtils.GetMultiResponseInputOutputWarning(outputPath);
                if (response == MultiResponse.Cancel)
                    return;
                if (response == MultiResponse.No)
                    continue;
                // response = MultiResponse.Yes or null
            }
            PrintLine(outputPath, 1, options.Quiet);
            string input = File.ReadAllText(fileName);
            string? output = BulkTransform.TryTransform(fileName, input, options.TransformOptions);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}