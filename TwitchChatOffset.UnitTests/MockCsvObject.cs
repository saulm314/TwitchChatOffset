using TwitchChatOffset.CommandLine.Options;
using static TwitchChatOffset.UnitTests.MockOptionAliases;

namespace TwitchChatOffset.UnitTests;

public class MockCsvObject
(
    long? _longObjectDefault,
    long? _longObjectStripped,
    long? _longObjectUnstripped,
    long _longObjectNonNullable,
    bool? _boolObject,
    char? _charObject,
    double? _doubleObject,
    MockEnum? _mockEnumObject,
    string? _stringObject
)
{
    [Aliases(["long-object-default", "longObjectDefault"])]
    public long? longObjectDefault = _longObjectDefault;

    [Aliases(typeof(MockOptionAliases), nameof(LongObjectStripped))]
    public long? longObjectStripped = _longObjectStripped;

    [Aliases(typeof(MockOptionAliases), nameof(LongObjectUnstripped), false)]
    public long? longObjectUnstripped = _longObjectUnstripped;

    [Aliases(typeof(MockOptionAliases), nameof(LongObjectNonNullable))]
    public long longObjectNonNullable = _longObjectNonNullable;

    [Aliases(typeof(MockOptionAliases), nameof(BoolObject))]
    public bool? boolObject = _boolObject;

    [Aliases(typeof(MockOptionAliases), nameof(CharObject))]
    public char? charObject = _charObject;

    [Aliases(typeof(MockOptionAliases), nameof(DoubleObject))]
    public double? doubleObject = _doubleObject;

    [Aliases(typeof(MockOptionAliases), nameof(MockEnumObject))]
    public MockEnum? mockEnumObject = _mockEnumObject;

    [Aliases(typeof(MockOptionAliases), nameof(StringObject))]
    public string? stringObject = _stringObject;

    public MockCsvObject() : this
    (
        default,
        default,
        default,
        default,
        default,
        default,
        default,
        default,
        default
    ) { }

    private (long?, long?, long?, long, bool?, char?, double?, MockEnum?, string?) EqualityFields =>
    (
        longObjectDefault,
        longObjectStripped,
        longObjectUnstripped,
        longObjectNonNullable,
        boolObject,
        charObject,
        doubleObject,
        mockEnumObject,
        stringObject
    );

    public override bool Equals(object? obj)
    {
        if (obj is not MockCsvObject other)
            return false;
        return EqualityFields == other.EqualityFields;
    }

    public override int GetHashCode() => EqualityFields.GetHashCode();
};