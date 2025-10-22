using System;

namespace TwitchChatOffset.CommandLine;

public static class EnumSerialization
{
    public static string Serialize<T>() where T : struct, Enum => '[' + string.Join('|', Enum.GetNames<T>()) + ']';
}