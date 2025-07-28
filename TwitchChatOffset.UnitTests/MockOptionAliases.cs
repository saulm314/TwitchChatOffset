using TwitchChatOffset.CommandLine.Options;

namespace TwitchChatOffset.UnitTests;

public static class MockOptionAliases
{
    public static AliasesContainer LongObjectStripped { get; } = new(["--long-object-stripped", "--long-stripped"]);
    public static AliasesContainer LongObjectUnstripped { get; } = new(["--long-object-unstripped", "--long-unstripped"]);
    public static AliasesContainer LongObjectNonNullable { get; } = new(["--long-object-non-nullable", "--long-non-nullable"]);
    public static AliasesContainer BoolObject { get; } = new(["--bool-object", "--bool"]);
    public static AliasesContainer CharObject { get; } = new(["--char-object", "--char"]);
    public static AliasesContainer DoubleObject { get; } = new(["--double-object", "--double"]);
    public static AliasesContainer MockEnumObject { get; } = new(["--mock-enum-object", "--mock-enum"]);
    public static AliasesContainer StringObject { get; } = new(["--string-object", "--string"]);
}