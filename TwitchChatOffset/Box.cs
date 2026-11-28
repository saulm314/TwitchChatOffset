namespace TwitchChatOffset;

public class Box<T>(T value)
{
    public T Value = value;

    public static implicit operator T(Box<T> box) => box.Value;
}