using System.CommandLine;

namespace TwitchChatOffset.CommandLine;

public static class PostBuildCommand
{
    public static readonly Command Command = new("post-build", "[Developer only] tasks that run immediately after the app builds") { Hidden = true };

    static PostBuildCommand()
    {
        Command.SetAction(Execute);
    }

    private static void Execute(ParseResult parseResult)
    {
    }
}