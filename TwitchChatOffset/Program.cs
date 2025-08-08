using TwitchChatOffset.CommandLine.Commands;

namespace TwitchChatOffset;

public class Program
{
    public static int Main(string[] args) => Root.Command.Parse(args).Invoke();
}
