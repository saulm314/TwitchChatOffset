global using static TwitchChatOffset.ConsoleUtils;
using TwitchChatOffset.CommandLine.Commands;
using System.CommandLine;
using TransformCommand = TwitchChatOffset.CommandLine.Commands.Transform;

namespace TwitchChatOffset;

internal class Program
{
    private static void Main(string[] args)
    {
        RootCommand rootCommand = new("Tools for handling Twitch chat JSON files");
        new TransformCommand().Add(rootCommand);
        new TransformManyToMany().Add(rootCommand);
        new TransformOneToMany().Add(rootCommand);
        new TransformAll().Add(rootCommand);
        rootCommand.Invoke(args);
    }
}
