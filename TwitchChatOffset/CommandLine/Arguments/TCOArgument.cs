namespace TwitchChatOffset.CommandLine.Arguments;

public class TCOArgument<TType>(string? name, string? description = null) : TCOArgumentBase<TType>(name, description);