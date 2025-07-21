using System;

namespace TwitchChatOffset;

public class JsonContentException : Exception
{
    public static JsonContentException ThrowEmpty() => new(Empty);
    public static JsonContentException ThrowNoComments() => new(NoComments);

    public const string Empty = "JSON content must not be empty";
    public const string NoComments = "comments object not found in JSON object";

    private JsonContentException(string? message) : base(message) { }
}