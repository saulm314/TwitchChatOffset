namespace TwitchChatOffset;

// ALWAYS TURN IT INTO A BOX BEFORE CONVERTING, TO ACCOUNT FOR T ITSELF BEING NULL
public readonly struct Box<T>(T value) where T : notnull
{
    private readonly T _value = value;

    public static implicit operator T(Box<T> box) => box._value;
    public static implicit operator Box<T>(T value) => new(value);
    public static implicit operator Box<T>?(T? value) => value is not null ? new(value) : null;

    public override bool Equals(object? obj)
    {
        if (obj is Box<T> box)
            return Equals(_value, box._value);
        if (obj is T t)
            return Equals(_value, t);
        return false;
    }

    public bool Equals(Box<T> box) => Equals(_value, box._value);
    public bool Equals(Box<T>? box) => box is not null && Equals(_value, box.Value._value);
    public bool Equals(T t) => Equals(_value, t);

    public override int GetHashCode() => _value.GetHashCode();

    public static bool operator ==(Box<T> left, Box<T> right) => Equals(left._value, right._value);
    public static bool operator ==(T left, Box<T> right) => Equals(left, right._value);
    public static bool operator ==(Box<T> left, T right) => Equals(left._value, right);
    public static bool operator ==(Box<T>? left, Box<T> right) => left is not null && Equals(left.Value._value, right._value);
    public static bool operator ==(Box<T> left, Box<T>? right) => right is not null && Equals(left._value, right.Value._value);
    public static bool operator ==(T left, Box<T>? right) => right is not null && Equals(left, right.Value._value);
    public static bool operator ==(Box<T>? left, T right) => left is not null && Equals(left.Value._value, right);
    public static bool operator ==(Box<T>? left, Box<T>? right)
        => (left is null && right is null) || (left is not null && right is not null && Equals(left.Value._value, right.Value._value));
    public static bool operator !=(Box<T> left, Box<T> right) => !Equals(left._value, right._value);
    public static bool operator !=(T left, Box<T> right) => !Equals(left, right._value);
    public static bool operator !=(Box<T> left, T right) => !Equals(left._value, right);
    public static bool operator !=(Box<T>? left, Box<T> right) => left is null || !Equals(left.Value._value, right._value);
    public static bool operator !=(Box<T> left, Box<T>? right) => right is null || !Equals(left._value, right.Value._value);
    public static bool operator !=(T left, Box<T>? right) => right is null || !Equals(left, right.Value._value);
    public static bool operator !=(Box<T>? left, T right) => left is null || !Equals(left.Value._value, right);
    public static bool operator !=(Box<T>? left, Box<T>? right)
        => (left is null ^ right is null) || (left is not null && right is not null && !Equals(left.Value._value, right.Value._value));
}