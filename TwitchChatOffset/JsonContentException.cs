using System;

namespace TwitchChatOffset;

public class JsonContentException : Exception
{
    public static JsonContentException Empty() => new(_Empty);
    public static JsonContentException NoComments() => new(_NoComments);
    public static JsonContentException NoContentOffsetSeconds(int i) => new(_NoContentOffsetSeconds + i);

    private const string _Empty = "JSON content must not be empty";
    private const string _NoComments = "comments object not found in JSON object";
    private const string _NoContentOffsetSeconds = "content_offset_seconds object not found in comments element ";

    private JsonContentException(string? message) : base(message) { }
}