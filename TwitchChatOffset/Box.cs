namespace TwitchChatOffset;

public readonly struct Box<T> where T : notnull
{
    // make constructor private to encourage consumer to use implicit operators instead
    private Box(T? value) => _value = value;

    // we make this field private to discourage the consumer from using it
    // and potentially confusing it with the System.Nullable<T>.Value property
    // which would return the box, rather than the value of the box
    private readonly T? _value;

    // the only implicit operator we are not implementing is T?(Box<T>?)
    // this is because T? is not the nullable version of T, but rather just T itself but with additional warnings
    // as such, if T is a struct, then T? is not a nullable type, but Box<T>? is,
    // so if the Box<T>? value is null, then there's no way to convert it to the non-nullable T?
    public static implicit operator T?(Box<T> box) => box._value;
    public static implicit operator Box<T>(T value) => new(value);
    public static implicit operator Box<T>?(T? value) => value is null ? null : new(value);

    // the == operator is always 100% reliable for determining equality
    // otherwise, calling any instance Equals method of a Box<T> or a Box<T>? is also 100% reliable
    // finally, calling object.Equals(object?, object?) with the first parameter being a Box<T> or Box<T>? or null is 100% reliable
    // no other method for determining equality should be used (e.g. instance Equals method of anything other than Box<T> or Box<T>?,
    //   object.Equals(object?, object?) with the first parameter not being Box<T>, Box<T>?, or null)

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

    public override int GetHashCode() => _value?.GetHashCode() ?? 0;
}