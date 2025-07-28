using TwitchChatOffset.CommandLine.Options;
using System;
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

    public void Deconstruct
    (
        out long? longObjectDefault,
        out long? longObjectStripped,
        out long? longObjectUnstripped,
        out long longObjectNonNullable,
        out bool? boolObject,
        out char? charObject,
        out double? doubleObject,
        out MockEnum? mockEnumObject,
        out string? stringObject
    )
    {
        longObjectDefault = this.longObjectDefault;
        longObjectStripped = this.longObjectStripped;
        longObjectUnstripped = this.longObjectUnstripped;
        longObjectNonNullable = this.longObjectNonNullable;
        boolObject = this.boolObject;
        charObject = this.charObject;
        doubleObject = this.doubleObject;
        mockEnumObject = this.mockEnumObject;
        stringObject = this.stringObject;
    }

    public static bool operator ==(MockCsvObject left, MockCsvObject right)
    {
        (
            long? longObjectDefaultLeft,
            long? longObjectStrippedLeft,
            long? longObjectUnstrippedLeft,
            long longObjectNonNullableLeft,
            bool? boolObjectLeft,
            char? charObjectLeft,
            double? doubleObjectLeft,
            MockEnum? mockEnumObjectLeft,
            string? stringObjectLeft
        ) = left;
        (
            long? longObjectDefaultRight,
            long? longObjectStrippedRight,
            long? longObjectUnstrippedRight,
            long longObjectNonNullableRight,
            bool? boolObjectRight,
            char? charObjectRight,
            double? doubleObjectRight,
            MockEnum? mockEnumObjectRight,
            string? stringObjectRight
        ) = right;
        return
            longObjectDefaultLeft == longObjectDefaultRight &&
            longObjectStrippedLeft == longObjectStrippedRight &&
            longObjectUnstrippedLeft == longObjectUnstrippedRight &&
            longObjectNonNullableLeft == longObjectNonNullableRight &&
            boolObjectLeft == boolObjectRight &&
            charObjectLeft == charObjectRight &&
            doubleObjectLeft == doubleObjectRight &&
            mockEnumObjectLeft == mockEnumObjectRight &&
            stringObjectLeft == stringObjectRight;
    }

    public static bool operator !=(MockCsvObject left, MockCsvObject right)
    {
        (
            long? longObjectDefaultLeft,
            long? longObjectStrippedLeft,
            long? longObjectUnstrippedLeft,
            long longObjectNonNullableLeft,
            bool? boolObjectLeft,
            char? charObjectLeft,
            double? doubleObjectLeft,
            MockEnum? mockEnumObjectLeft,
            string? stringObjectLeft
        ) = left;
        (
            long? longObjectDefaultRight,
            long? longObjectStrippedRight,
            long? longObjectUnstrippedRight,
            long longObjectNonNullableRight,
            bool? boolObjectRight,
            char? charObjectRight,
            double? doubleObjectRight,
            MockEnum? mockEnumObjectRight,
            string? stringObjectRight
        ) = right;
        return
            longObjectDefaultLeft != longObjectDefaultRight ||
            longObjectStrippedLeft != longObjectStrippedRight ||
            longObjectUnstrippedLeft != longObjectUnstrippedRight ||
            longObjectNonNullableLeft != longObjectNonNullableRight ||
            boolObjectLeft != boolObjectRight ||
            charObjectLeft != charObjectRight ||
            doubleObjectLeft != doubleObjectRight ||
            mockEnumObjectLeft != mockEnumObjectRight ||
            stringObjectLeft != stringObjectRight;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        return this == (MockCsvObject)obj;
    }

    public override int GetHashCode()
    {
        (
            long? longObjectDefault,
            long? longObjectStripped,
            long? longObjectUnstripped,
            long longObjectNonNullable,
            bool? boolObject,
            char? charObject,
            double? doubleObject,
            MockEnum? mockEnumObject,
            string? stringObject
        ) = this;
        return
            (longObjectDefault?.GetHashCode() ?? hash) ^
            (longObjectStripped?.GetHashCode() ?? hash) ^
            (longObjectUnstripped?.GetHashCode() ?? hash) ^
            longObjectNonNullable.GetHashCode() ^
            (boolObject?.GetHashCode() ?? hash) ^
            (charObject?.GetHashCode() ?? hash) ^
            (doubleObject?.GetHashCode() ?? hash) ^
            (mockEnumObject?.GetHashCode() ?? hash) ^
            (stringObject?.GetHashCode() ?? hash);
    }

    private static int hash = Guid.NewGuid().GetHashCode();
};