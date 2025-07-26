using System;

namespace TwitchChatOffset;

public class JsonContentException : Exception
{
    public static JsonContentException ThrowEmpty() => new(Empty);
    public static JsonContentException ThrowNoComments() => new(NoComments);
    public static JsonContentException ThrowNoContentOffsetSeconds(int i) => new(NoContentOffsetSeconds(i));

    public const string Empty = "JSON content must not be empty";
    public const string NoComments = "comments object not found in JSON object";
    public static string NoContentOffsetSeconds(int i) => _NoContentOffsetSeconds + i;

    private const string _NoContentOffsetSeconds = "content_offset_seconds object not found in comments element ";

    private JsonContentException(string? message) : base(message) { }
}