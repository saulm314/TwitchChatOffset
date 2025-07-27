using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public static class Transform
{
    // any negative end value represents infinity (no end)
    public static string MTransform(string inputString, long start, long end, Format format)
    {
        JToken input = (JToken)(JsonConvert.DeserializeObject(inputString) ?? throw JsonContentException.Empty());
        ApplyOffset(input, start, end);
        return ApplyFormat(input, format);
    }

    // any negative end value represents infinity (no end)
    // this method does not modify the input JToken provided in the arguments
    public static string MTransform(JToken input, long start, long end, Format format)
    {
        input = input.DeepClone();
        ApplyOffset(input, start, end);
        return ApplyFormat(input, format);
    }

    private static void ApplyOffset(JToken parent, long start, long end)
    {
        if (start == 0 && end < 0)
            return;
        JArray comments = (JArray)(parent["comments"] ?? throw JsonContentException.NoComments());
        int i = 0;
        int originalIndex = -1;
        while (i < comments.Count)
        {
            originalIndex++;
            JToken comment = comments[i];
            JValue commentOffset = (JValue)(comment["content_offset_seconds"] ?? throw JsonContentException.NoContentOffsetSeconds(originalIndex));
            long commentOffsetValue = (long)commentOffset.Value!;
            if (commentOffsetValue < start)
            {
                comments.RemoveAt(i);
                continue;
            }
            if (end >= 0 && commentOffsetValue > end)
            {
                comments.RemoveAt(i);
                continue;
            }
            commentOffset.Value = commentOffsetValue - start;
            i++;
        }
    }

    private static string ApplyFormat(JToken parent, Format format)
    {
        return format switch
        {
            Format.Json => ApplyFormatJson(parent),
            Format.JsonIndented => ApplyFormatJsonIndented(parent),
            Format.Plaintext => ApplyFormatPlaintext(parent),
            _ => throw new InternalException("Internal error: unrecognised format type")
        };
    }

    private static string ApplyFormatJson(JToken parent)
    {
        return JsonConvert.SerializeObject(parent);
    }

    private static string ApplyFormatJsonIndented(JToken parent)
    {
        return JsonConvert.SerializeObject(parent, Formatting.Indented);
    }

    private static string ApplyFormatPlaintext(JToken parent)
    {
        StringBuilder stringBuilder = new();
        JArray comments = (JArray)(parent["comments"] ?? throw JsonContentException.NoComments());
        for (int i = 0; i < comments.Count; i++)
        {
            JToken comment = comments[i];
            JValue commentOffset = (JValue)(comment["content_offset_seconds"] ?? throw JsonContentException.NoContentOffsetSeconds(i));
            long commentOffsetValue = (long)commentOffset.Value!;
            TimeSpan timeSpan = TimeSpan.FromSeconds(commentOffsetValue);
            stringBuilder.Append(timeSpan);
            stringBuilder.Append(' ');

            JValue commenter = (JValue)(comment["commenter"] ?? throw JsonContentException.NoCommenter(i));
            JValue displayName = (JValue)(commenter["display_name"] ?? throw JsonContentException.NoDisplayName(i));
            string displayNameValue = (string)displayName.Value!;
            stringBuilder.Append(displayNameValue);
            stringBuilder.Append(": ");

            JValue message = (JValue)(comment["message"] ?? throw JsonContentException.NoMessage(i));
            JValue body = (JValue)(message["body"] ?? throw JsonContentException.NoBody(i));
            string bodyValue = (string)body.Value!;
            stringBuilder.Append(bodyValue);
            stringBuilder.Append('\n');
        }
        return stringBuilder.ToString();
    }
}