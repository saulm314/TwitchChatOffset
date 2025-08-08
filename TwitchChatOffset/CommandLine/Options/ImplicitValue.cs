namespace TwitchChatOffset.CommandLine.Options;

public readonly record struct ImplicitValue<T>(T Value, bool Implicit)
{
    public static implicit operator T(ImplicitValue<T> implicitValue) => implicitValue.Value;
}