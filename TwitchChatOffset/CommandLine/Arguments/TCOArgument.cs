namespace TwitchChatOffset.CommandLine.Arguments;

public class TCOArgument<T>(string? name, string? description = null) : TCOArgumentBase<T>(name, description);