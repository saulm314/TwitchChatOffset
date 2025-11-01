namespace TwitchChatOffset.Options;

// a value that is either implicit or explicit
public readonly record struct Plicit<T>(T Value, bool Implicit)
{
    public static implicit operator T(Plicit<T> plicit) => plicit.Value;
}