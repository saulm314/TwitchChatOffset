using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
using System.CommandLine;
using System.IO;
using static TwitchChatOffset.CommandLine.Arguments;

namespace TwitchChatOffset.CommandLine;

public static class TransformCommand
{
    public static readonly Command Command = new("transform", "Transform a JSON Twitch chat file and put the new contents in a new file");

    static TransformCommand()
    {
        Command.Add(InputArgument);
        Command.Add(OutputArgument);
        IOptionGroup.AddCliOptions<TransformOptions>(Command);
        Command.SetAction(Execute);
    }

    private static void Execute(ParseResult parseResult)
    {
        string inputPath = parseResult.GetValue(InputArgument)!;
        string outputPath = parseResult.GetValue(OutputArgument)!;
        TransformOptions options = IOptionGroup.ParseOptions<TransformOptions>(parseResult);
        string input = File.ReadAllText(inputPath);
        string output = Transform.DoTransform(input, options);
        File.WriteAllText(outputPath, output);
    }
}