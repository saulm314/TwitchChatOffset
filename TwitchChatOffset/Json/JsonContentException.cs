// MIT License: https://gist.github.com/saulm314/91f0d83ce1a931b5086169e17b6e4eb0

using System;

namespace TwitchChatOffset.Json;

public abstract class JsonContentException(string? message) : Exception(message)
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

    public class InvalidValueAssignment<T>(string path) : InvalidValueAssignment(typeof(T), path);

    public class InvalidValueAssignment(Type type, string path)
        : JsonContentException($"Cannot assign a value of type {type.FullName} to JValue at {path}".AddPathWarning())
    {
        public Type PType => type;
        public string Path => path;
    }

    public class OutOfRange(string path, int index)
        : JsonContentException($"Index {index} out of range of JArray at {path}".AddPathWarning())
    {
        public string Path => path;
        public int Index => index;
    }
}