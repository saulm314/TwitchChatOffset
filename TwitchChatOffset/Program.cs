global using static TwitchChatOffset.ConsoleUtils.ConsoleOutput;
using TwitchChatOffset.CommandLine;
using System.Globalization;

namespace TwitchChatOffset;

public class Program
{
    public static int Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        return RootCommand.Command.Parse(args).Invoke();
    }
}
