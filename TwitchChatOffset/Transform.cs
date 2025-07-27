using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public static class Transform
{
    // any negative end value represents infinity (no end)
    public static string MTransform(string input, long start, long end, Format format)
    {
        JToken parent = (JToken)(JsonConvert.DeserializeObject(input) ?? throw JsonContentException.Empty());
        return MTransform(parent, start, end, format);
    }

    // any negative end value represents infinity (no end)
    public static string MTransform(JToken input, long start, long end, Format format)
    {
        ApplyOffset(input, start, end);
        return ApplyFormat(input, format);
    }

    private static void ApplyOffset(JToken parent, long start, long end)
    {
        if (start == 0 && end < 0)
            return;
        JArray comments = (JArray)(parent["comments"] ?? throw JsonContentException.NoComments());
        int i = 0;
        int globalCount = -1;
        while (i < comments.Count)
        {
            globalCount++;
            JToken comment = comments[i];
            JValue commentOffset = (JValue)(comment["content_offset_seconds"] ?? throw JsonContentException.NoContentOffsetSeconds(globalCount));
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
        foreach (JToken comment in comments)
        {
            JValue commentOffset = (JValue)comment["content_offset_seconds"]!;
            long commentOffsetValue = (long)commentOffset.Value!;
            TimeSpan timeSpan = TimeSpan.FromSeconds(commentOffsetValue);
            stringBuilder.Append(timeSpan);
            stringBuilder.Append(' ');

            JValue displayName = (JValue)comment["commenter"]!["display_name"]!;
            string displayNameValue = (string)displayName.Value!;
            stringBuilder.Append(displayNameValue);
            stringBuilder.Append(": ");

            JValue messageBody = (JValue)comment["message"]!["body"]!;
            string messageBodyValue = (string)messageBody.Value!;
            stringBuilder.Append(messageBodyValue);
            stringBuilder.Append('\n');
        }
        return stringBuilder.ToString();
    }
}