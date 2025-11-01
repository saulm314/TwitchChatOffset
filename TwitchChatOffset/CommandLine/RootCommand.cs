﻿namespace TwitchChatOffset.CommandLine;

public static class RootCommand
{
    static RootCommand()
    {
        Command.Add(TransformCommand.Command);
    }

    public static readonly System.CommandLine.RootCommand Command = new("Tools for handling Twitch chat JSON files");
}