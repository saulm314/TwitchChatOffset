using System.CommandLine;
using static TwitchChatOffset.CommandLine.Arguments;

namespace TwitchChatOffset.CommandLine;

public static class TransformCommand
{
    public static readonly Command Command = new("transform", "Transform a JSON Twitch chat file and put the new contents in a new file");

    static TransformCommand()
    {
        Command.Add(InputArgument);
        Command.Add(OutputArgument);
    }
}