namespace TwitchChatOffset.Options;

// a value that is either implicit or explicit
public record struct Plicit<T>(T Value, bool Explicit) : IPlicit
{
    public static implicit operator T(Plicit<T> plicit) => plicit.Value;
}