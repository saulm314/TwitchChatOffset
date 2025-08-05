using System;

namespace TwitchChatOffset.Csv;

public abstract class CsvSerializationInternalException(string? message) : InternalException(message)
{
    public class DuplicateAlias<T>(string alias) : DuplicateAlias(alias, typeof(T));

    public class DuplicateAlias(string alias, Type type)
        : CsvSerializationInternalException($"Internal error: duplicate alias {alias} found in type {type.FullName}")
    {
        public string Alias => alias;
        public Type PType => type;
    }
}