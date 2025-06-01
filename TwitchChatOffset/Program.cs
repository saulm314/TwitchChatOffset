using TwitchChatOffset.Commands;
using System.CommandLine;

namespace TwitchChatOffset;

internal class Program
{
    private static void Main(string[] args)
    {
        RootCommand rootCommand = new("Tools for handling Twitch chat JSON files");
        new Transform().Add(rootCommand);
        new TransformManyToMany().Add(rootCommand);
        new TransformOneToMany().Add(rootCommand);
        new TransformAll().Add(rootCommand);
        rootCommand.Invoke(args);
    }
}
