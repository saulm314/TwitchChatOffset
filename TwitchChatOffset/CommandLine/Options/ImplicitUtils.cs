using System.CommandLine;

namespace TwitchChatOffset.CommandLine.Options;

public static class ImplicitUtils
{
    public static ImplicitValue<T> GetImplicit<T>(this ParseResult parseResult, Option<T> option)
        => new(parseResult.GetValue(option)!, parseResult.GetResult(option)!.Implicit);
}