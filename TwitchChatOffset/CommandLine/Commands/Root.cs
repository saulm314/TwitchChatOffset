using System.CommandLine;

namespace TwitchChatOffset.CommandLine.Commands;

public static class Root
{
    static Root()
    {
        Command.Add(TransformCommand.Command);
        Command.Add(TransformManyToMany.Command);
        Command.Add(TransformOneToMany.Command);
        Command.Add(TransformAll.Command);
    }

    public static readonly RootCommand Command = new("Tools for handling Twitch chat JSON files");
}