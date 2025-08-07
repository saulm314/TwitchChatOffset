namespace TwitchChatOffset;

public readonly struct Box<T> where T : notnull
{
    private Box(T value) => _value = value;

    private readonly T _value;

    public static implicit operator T(Box<T> box) => box._value;
    public static implicit operator Box<T>(T value) => new(value);
    public static implicit operator Box<T>?(T? value) => value is null ? null : new(value);

    public override bool Equals(object? obj)
    {
        if (obj is Box<T> box)
            return Equals(_value, box._value);
        if (obj is T t)
            return Equals(_value, t);
        if (obj is null)
            return _value is null;
        return false;
    }

    public bool Equals(Box<T> box) => Equals(_value, box._value);
    public bool Equals(Box<T>? box) => (_value is null && box is null) || (box is not null && Equals(_value, box.Value._value));
    public bool Equals(T? t) => Equals(_value, t);

    public static bool operator ==(Box<T> left, Box<T> right) => left.Equals(right);
    public static bool operator ==(T? left, Box<T> right) => right.Equals(left);
    public static bool operator ==(Box<T> left, T? right) => left.Equals(right);
    public static bool operator ==(Box<T>? left, Box<T> right) => right.Equals(left);
    public static bool operator ==(Box<T> left, Box<T>? right) => left.Equals(right);
    public static bool operator ==(T? left, Box<T>? right) => (left is null && right is null) || (right is not null && ((Box<T>)right).Equals(left));
    public static bool operator ==(Box<T>? left, T? right) => (left is null && right is null) || (left is not null && ((Box<T>)left).Equals(right));
    public static bool operator ==(Box<T>? left, Box<T>? right) => (left is null && right is null) || (left is not null && ((Box<T>)left).Equals(right));
    public static bool operator !=(Box<T> left, Box<T> right) => !(left == right);
    public static bool operator !=(T? left, Box<T> right) => !(left == right);
    public static bool operator !=(Box<T> left, T? right) => !(left == right);
    public static bool operator !=(Box<T>? left, Box<T> right) => !(left == right);
    public static bool operator !=(Box<T> left, Box<T>? right) => !(left == right);
    public static bool operator !=(T? left, Box<T>? right) => !(left == right);
    public static bool operator !=(Box<T>? left, T? right) => !(left == right);
    public static bool operator !=(Box<T>? left, Box<T>? right) => !(left == right);

    public override int GetHashCode() => _value.GetHashCode();
}