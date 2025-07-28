using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public static class Transform
{
    public static string MTransform(string inputString, long start, long end, Format format)
    {
        JToken json = (JToken)(JsonConvert.DeserializeObject(inputString) ?? throw JsonContentException.Empty());
        ApplyOffset(json, start, end);
        return ApplyFormat(json, format);
    }

    public static string MTransform(JToken inputJson, long start, long end, Format format)
    {
        JToken json = inputJson.DeepClone();
        ApplyOffset(json, start, end);
        return ApplyFormat(json, format);
    }

    public static void ApplyOffset(JToken json, long start, long end)
    {
        if (start == 0 && end < 0)
            return;
        if (end < start)
            PrintWarning("Warning: end value is less than start value, so all comments will get deleted");
        JArray comments = (JArray)(json["comments"] ?? throw JsonContentException.NoComments());
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

    public static string ApplyFormat(JToken json, Format format)
    {
        return format switch
        {
            Format.Json => ApplyFormatJson(json),
            Format.JsonIndented => ApplyFormatJsonIndented(json),
            Format.Plaintext => ApplyFormatPlaintext(json),
            _ => throw new InternalException("Internal error: unrecognised format type")
        };
    }

    public static string ApplyFormatJson(JToken json)
    {
        return JsonConvert.SerializeObject(json);
    }

    public static string ApplyFormatJsonIndented(JToken json)
    {
        return JsonConvert.SerializeObject(json, Formatting.Indented);
    }

    public static string ApplyFormatPlaintext(JToken json)
    {
        StringBuilder stringBuilder = new();
        JArray comments = (JArray)(json["comments"] ?? throw JsonContentException.NoComments());
        for (int i = 0; i < comments.Count; i++)
        {
            JToken comment = comments[i];
            JValue commentOffset = (JValue)(comment["content_offset_seconds"] ?? throw JsonContentException.NoContentOffsetSeconds(i));
            long commentOffsetValue = (long)commentOffset.Value!;
            TimeSpan timeSpan = TimeSpan.FromSeconds(commentOffsetValue);
            stringBuilder.Append(timeSpan);
            stringBuilder.Append(' ');

            JToken commenter = comment["commenter"] ?? throw JsonContentException.NoCommenter(i);
            JValue displayName = (JValue)(commenter["display_name"] ?? throw JsonContentException.NoDisplayName(i));
            string displayNameValue = (string)displayName.Value!;
            stringBuilder.Append(displayNameValue);
            stringBuilder.Append(": ");

            JToken message = comment["message"] ?? throw JsonContentException.NoMessage(i);
            JValue body = (JValue)(message["body"] ?? throw JsonContentException.NoBody(i));
            string bodyValue = (string)body.Value!;
            stringBuilder.Append(bodyValue);
            stringBuilder.Append('\n');
        }
        return stringBuilder.ToString();
    }
}