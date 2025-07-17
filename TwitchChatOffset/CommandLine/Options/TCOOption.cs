using System;

namespace TwitchChatOffset.CommandLine.Options;

public class TCOOption<TType>(string[] aliases, Func<TType> getDefaultValue, string? description = null)
    : TCOOptionBase<TType>(aliases, getDefaultValue, description);