using TwitchChatOffset.CommandLine;

namespace TwitchChatOffset;

public class Program
{
    public static int Main(string[] args) => RootCommand.Command.Parse(args).Invoke();
}
