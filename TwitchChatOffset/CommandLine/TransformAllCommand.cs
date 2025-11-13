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
        PrintError(options.Response.Value);
        string[] inputPaths = Directory.GetFiles(options.InputDir, options.SearchPattern);
        PrintEnumerable(inputPaths, "Input files found:", 0, options.Quiet);
        _ = Directory.CreateDirectory(options.OutputDir);
        MultiResponse? globalResponse = null;
        PrintLine("Writing files...", 0, options.Quiet);
        foreach (string inputPath_ in inputPaths)
        {
            string inputPath = inputPath_;
            string outputFileName = options.Suffix == "/auto" ? Path.GetFileName(inputPath) : Path.GetFileNameWithoutExtension(inputPath) + options.Suffix;
            string outputPath = Path.Combine(options.OutputDir, outputFileName);
            Response? response = ResponseUtils.ValidateInputOutput(ref inputPath, ref outputPath, ref globalResponse, options.Response);
            if (response == null)
                return;
            if (response == Response.No)
                continue;
            PrintLine(outputPath, 1, options.Quiet);
            string input = File.ReadAllText(inputPath);
            string? output = BulkTransform.TryTransform(inputPath, input, options.TransformOptions);
            if (output == null)
                continue;
            File.WriteAllText(outputPath, output);
        }
    }
}