using System;

namespace TwitchChatOffset.CSV;

public class CsvSerializationInternalException : InternalException
{
    public static CsvSerializationInternalException DuplicateAliases(string alias, Type type)
        => new(_DuplicateAliases + $"{alias} (type {type.FullName})");

    private const string _DuplicateAliases = "Internal error: duplicate aliases found in type to deserialize CSV into: ";

    private CsvSerializationInternalException(string? message) : base(message) { }
}