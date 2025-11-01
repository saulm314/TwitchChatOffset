using TwitchChatOffset.Json;
using TwitchChatOffset.Options.Groups;
using TwitchChatOffset.Ytt;
using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public static class Transform
{
    public static string DoTransform(string inputString, TransformOptions options)
    {
        JObject json = JsonUtils.Deserialize(inputString);
        ApplyOffset(json, options);
        return Serialize(json, options);
    }

    public static string DoTransform(JToken inputJson, TransformOptions options)
    {
        JToken json = inputJson.DeepClone();
        ApplyOffset(json, options);
        return Serialize(json, options);
    }

    //_______________________________________________________________________

    public static void ApplyOffset(JToken json, TransformOptions options)
    {
        (long start, long end, long delay) = options;
        if (delay < 0)
        {
            PrintWarning("Warning: delay value is less than zero which is not supported; treating it as zero instead");
            delay = 0;
        }
        if (end >= 0 && end < start)
            PrintWarning("Warning: end value is less than start value, so all comments will get deleted");
        if (start == 0 && end < 0 && delay == 0)
            return;
        JArray comments = json.D("comments").As<JArray>();
        int i = 0;
        while (i < comments.Count)
        {
            JValue offsetJValue = comments[i].D("content_offset_seconds").As<JValue>();
            long offset = offsetJValue.As<long>();
            bool inRange = offset >= start && (offset <= end || end < 0);
            if (!inRange)
            {
                comments.RemoveAt(i);
                continue;
            }
            offsetJValue.Set(offset - start + delay);
            i++;
        }
    }

    public static string Serialize(JToken json, TransformOptions options)
    {
        return options.Format.Value switch
        {
            Format.Json => SerializeToJson(json),
            Format.JsonIndented => SerializeToJsonIndented(json),
            Format.Ytt => SerializeToYtt(json, options.SubtitleOptions),
            Format.Plaintext => SerializeToPlaintext(json),
            _ => throw new InternalException("Internal error: unrecognised format type")
        };
    }

    //_____________________________________________________________________

    public static string SerializeToJson(JToken json)
    {
        return JsonConvert.SerializeObject(json);
    }

    public static string SerializeToJsonIndented(JToken json)
    {
        return JsonConvert.SerializeObject(json, Formatting.Indented);
    }

    public static string SerializeToYtt(JToken json, SubtitleOptions options)
    {
        return YttSerialization.Serialize(json, options);
    }

    public static string SerializeToPlaintext(JToken json)
    {
        StringBuilder builder = new();
        JArray comments = json.D("comments").As<JArray>();
        foreach (JToken comment in comments)
        {
            long offset = comment.D("content_offset_seconds").As<long>();
            TimeSpan timeSpan = TimeSpan.FromSeconds(offset);
            string displayName = comment.D("commenter").D("display_name").As<string>();
            string message = comment.D("message").D("body").As<string>();

            builder.Append(timeSpan);
            builder.Append(' ');
            builder.Append(displayName);
            builder.Append(": ");
            builder.Append(message);
            builder.Append('\n');
        }
        return builder.ToString();
    }
}