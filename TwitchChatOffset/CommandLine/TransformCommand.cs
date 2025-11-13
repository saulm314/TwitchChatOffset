using TwitchChatOffset.ConsoleUtils;
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
        Command.AddOptions<TransformOptions>();
        Command.SetAction(Execute);
    }

    private static void Execute(ParseResult parseResult)
    {
        string inputPath = parseResult.GetValue(InputArgument)!;
        string outputPath = parseResult.GetValue(OutputArgument)!;
        TransformOptions options = parseResult.ParseOptions<TransformOptions>();
        if (inputPath == outputPath)
        {
            Response response = ResponseUtils.GetResponseInputOutputWarning(outputPath);
            if (response == Response.No)
                return;
            // response = Response.Yes
        }
        string input = File.ReadAllText(inputPath);
        string output = Transform.DoTransform(input, options);
        File.WriteAllText(outputPath, output);
    }
}