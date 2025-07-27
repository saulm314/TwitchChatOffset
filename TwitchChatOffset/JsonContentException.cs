using System;

namespace TwitchChatOffset;

public class JsonContentException : Exception
{
    public static JsonContentException Empty() => new(_Empty);
    public static JsonContentException NoComments() => new(_NoComments);
    public static JsonContentException NoContentOffsetSeconds(int index) => new(_NoContentOffsetSeconds + index);
    public static JsonContentException NoCommenter(int index) => new(_NoCommenter + index);
    public static JsonContentException NoDisplayName(int index) => new(_NoDisplayName + index);
    public static JsonContentException NoMessage(int index) => new(_NoMessage + index);
    public static JsonContentException NoBody(int index) => new(_NoBody + index);

    private const string _Empty = "JSON content must not be empty";
    private const string _NoComments = "comments object not found in JSON object";
    private const string _NoContentOffsetSeconds = "content_offset_seconds object not found in comments element ";
    private const string _NoCommenter = "commenter object not found in comments element ";
    private const string _NoDisplayName = "display_name object not found in commenter object in comments element ";
    private const string _NoMessage = "message object not found in comments element ";
    private const string _NoBody = "body object not found in message object in comments element ";

    private JsonContentException(string? message) : base(message) { }
}