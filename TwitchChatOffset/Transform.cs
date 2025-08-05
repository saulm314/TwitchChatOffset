using TwitchChatOffset.Json;
using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public static class Transform
{
    public static string MTransform(string inputString, long start, long end, Format format)
    {
        JObject json = JsonUtils.Deserialize(inputString);
        ApplyOffset(json, start, end);
        return Serialize(json, format);
    }

    public static string MTransform(JToken inputJson, long start, long end, Format format)
    {
        JToken json = inputJson.DeepClone();
        ApplyOffset(json, start, end);
        return Serialize(json, format);
    }

    //_______________________________________________________________________

    public static void ApplyOffset(JToken json, long start, long end)
    {
        if (start == 0 && end < 0)
            return;
        if (end >= 0 && end < start)
            PrintWarning("Warning: end value is less than start value, so all comments will get deleted");
        JArray comments = json.D<JArray>("comments");
        int i = 0;
        while (i < comments.Count)
        {
            JValue offsetJValue = comments[i].D<JValue>("content_offset_seconds");
            long offset = offsetJValue.As<long>();
            bool inRange = offset >= start && (offset <= end || end < 0);
            if (!inRange)
            {
                comments.RemoveAt(i);
                continue;
            }
            offsetJValue.Set(offset - start);
            i++;
        }
    }

    public static string Serialize(JToken json, Format format)
    {
        return format switch
        {
            Format.Json => SerializeToJson(json),
            Format.JsonIndented => SerializeToJsonIndented(json),
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

    public static string SerializeToPlaintext(JToken json)
    {
        StringBuilder builder = new();
        JArray comments = json.D<JArray>("comments");
        for (int i = 0; i < comments.Count; i++)
        {
            long offset = comments[i].D<long>("content_offset_seconds");
            TimeSpan timeSpan = TimeSpan.FromSeconds(offset);
            string displayName = comments[i].D("commenter").D<string>("display_name");
            string message = comments[i].D("message").D<string>("body");

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