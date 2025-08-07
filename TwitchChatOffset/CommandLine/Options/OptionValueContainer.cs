namespace TwitchChatOffset.CommandLine.Options;

public readonly record struct OptionValueContainer<T>(T Value, bool Explicit)
{
    public static implicit operator T(OptionValueContainer<T> container) => container.Value;
}