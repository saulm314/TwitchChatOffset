namespace TwitchChatOffset;

public readonly record struct Box<T>(T Value) where T : notnull
{
    public static implicit operator T(Box<T> box) => box.Value;
    public static implicit operator Box<T>(T value) => new(value);
    public static implicit operator Box<T>?(T? value) => value is not null ? new(value) : null;
}