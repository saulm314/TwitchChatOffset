using System;

namespace TwitchChatOffset.Json;

public abstract class JsonContentException : Exception
{
    public class Empty() : JsonContentException("base JSON object { } not found");

    public class PropertyNotFound(string path, string propertyName)
        : JsonContentException($"JSON property {path.Dereference(propertyName)} not found".AddPathWarning())
    {
        public string Path => path;
        public string PropertyName => propertyName;
    }

    public class InvalidConversion<T>(string path, object? value) : InvalidConversion(typeof(T), path, value);

    public class InvalidConversion(Type desiredType, string path, object? value)
        : JsonContentException($"""
            Could not convert value:
            {value}
            in JSON path:
            {path}
            to type:
            {desiredType.FullName}
            """.AddPathWarning())
    {
        public Type DesiredType => desiredType;
        public string Path => Path;
        public object? Value => value;
    }

    private JsonContentException(string? message) : base(message) { }
}