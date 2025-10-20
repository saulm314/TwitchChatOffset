// MIT License: https://gist.github.com/saulm314/bf4c6cd9e4a9b045b52ad123a80a5c39

namespace TwitchChatOffset;

public readonly record struct Wrap<T>(T Value)
{
    public static implicit operator Wrap<T>(T value) => new(value);
    public static implicit operator Wrap<T>?(T? value) => value is null ? null : new(value);
}