using System;

namespace TwitchChatOffset;

// new values should never be made by calling a constructor or calling the default() function, but by using implicit operators only
public readonly struct Box<T>
{
    // make constructor private to encourage consumer to use implicit operators instead
    private Box(T value) => _value = value;

    // we do not declare the _value field as a T?, because if Box<T> instances are only ever made with implicit operators,
    // then _value is guaranteed to never be null
    // however when checking for equality, we assume that something might have gone wrong anyway and treat _value as if it could be null

    // we make this field private to discourage the consumer from using it
    // and potentially confusing it with the System.Nullable<T>.Value property
    // which would return the box, rather than the value of the box
    private readonly T _value;

    // the only implicit operator we are not implementing is T?(Box<T>?)
    // this is because T? is not the nullable version of T, but rather just T itself but with additional warnings
    // as such, if T is a struct, then T? is not a nullable type, but Box<T>? is,
    // so if the Box<T>? value is null, then there's no way to convert it to the non-nullable T?
    public static implicit operator T(Box<T> box) => box._value;
    public static implicit operator Box<T>(T value) => new(value);
    public static implicit operator Box<T>?(T? value) => value is null ? null : new(value);

    // Reliable methods of determining equality:
    // * the == operator, if either the left or the right expression is a Box<T>, Box<T>? at compile time
    // * any instance Equals method of a Box<T> or a Box<T>?
    // * the object.Equals(object?, object?) method but only if the first parameter is a Box<T>, Box<T>?, or null

    // Unreliable methods of determining equality (do not use these!):
    // * the == operator when neither expression is a Box<T> or Box<T>? at compile time
    //     * e.g. if the left and right expressions are object? at compile time, the comparison will fail even if both are Box<T> at runtime
    //     * this is because in that case their references will be compared, but Box<T> is a struct so we care about the value, not the reference
    // * the is null operator (use == null instead!)
    //     * on the other hand, the is operator to determine type is fine, e.g. is Box<T>
    // * an instance Equals method of a type other than Box<T> or Box<T>?
    // * the object.Equals(object?, object?) method when the first parameter isn't a Box<T>, Box<T>?, or null
    // * the object.ReferenceEquals(object?, object?) method
    // * 

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
    public static bool operator ==(object? left, Box<T> right) => right.Equals(left);
    public static bool operator ==(Box<T> left, object? right) => left.Equals(right);
    public static bool operator ==(object? left, Box<T>? right) => (left is null && right is null) || (right is not null && ((Box<T>)right).Equals(left));
    public static bool operator ==(Box<T>? left, object? right) => (left is null && right is null) || (left is not null && ((Box<T>)left).Equals(right));
    public static bool operator ==(ValueType? left, Box<T> right) => right.Equals(left);
    public static bool operator ==(Box<T> left, ValueType? right) => left.Equals(right);
    public static bool operator ==(ValueType? left, Box<T>? right) => (left is null && right is null) || (right is not null && ((Box<T>)right).Equals(left));
    public static bool operator ==(Box<T>? left, ValueType? right) => (left is null && right is null) || (left is not null && ((Box<T>)left).Equals(right));
    public static bool operator !=(Box<T> left, Box<T> right) => !(left == right);
    public static bool operator !=(T? left, Box<T> right) => !(left == right);
    public static bool operator !=(Box<T> left, T? right) => !(left == right);
    public static bool operator !=(Box<T>? left, Box<T> right) => !(left == right);
    public static bool operator !=(Box<T> left, Box<T>? right) => !(left == right);
    public static bool operator !=(T? left, Box<T>? right) => !(left == right);
    public static bool operator !=(Box<T>? left, T? right) => !(left == right);
    public static bool operator !=(Box<T>? left, Box<T>? right) => !(left == right);
    public static bool operator !=(object? left, Box<T> right) => !(left == right);
    public static bool operator !=(Box<T> left, object? right) => !(left == right);
    public static bool operator !=(object? left, Box<T>? right) => !(left == right);
    public static bool operator !=(Box<T>? left, object? right) => !(left == right);
    public static bool operator !=(ValueType? left, Box<T> right) => !(left == right);
    public static bool operator !=(Box<T> left, ValueType? right) => !(left == right);
    public static bool operator !=(ValueType? left, Box<T>? right) => !(left == right);
    public static bool operator !=(Box<T>? left, ValueType? right) => !(left == right);

    public override int GetHashCode() => _value?.GetHashCode() ?? 0;
}