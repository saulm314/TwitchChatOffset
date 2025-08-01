using System;

namespace TwitchChatOffset.CommandLine.Options;

public class TCOOption<T>(string[] aliases, Func<T> getDefaultValue, string? description = null) : TCOOptionBase<T>(aliases, getDefaultValue, description);