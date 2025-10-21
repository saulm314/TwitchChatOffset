using TwitchChatOffset.Json;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YTSubConverter.Shared;
using YTSubConverter.Shared.Formats;

namespace TwitchChatOffset;

public static class Transform
{
    public static string DoTransform(string inputString, long start, long end, Format format)
    {
        JObject json = JsonUtils.Deserialize(inputString);
        ApplyOffset(json, start, end);
        return Serialize(json, format);
    }

    public static string DoTransform(JToken inputJson, long start, long end, Format format)
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
            offsetJValue.Set(offset - start);
            i++;
        }
    }

    public static string Serialize(JToken json, Format format)
    {
        return format switch
        {
            Format.json => SerializeToJson(json),
            Format.jsonindented => SerializeToJsonIndented(json),
            Format.ytt => SerializeToYtt(json),
            Format.plaintext => SerializeToPlaintext(json),
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

    public static string SerializeToYtt(JToken json)
    {
        YttDocument ytt = new();
        JArray comments = json.D("comments").As<JArray>();
        foreach (JToken comment in comments)
        {
            long offset = comment.D("content_offset_seconds").As<long>();
            TimeSpan timeSpan = TimeSpan.FromSeconds(offset);
            DateTime dateTime = SubtitleDocument.TimeBase + timeSpan;
            DateTime dateTimeEnd = dateTime.AddSeconds(2);
            string displayName = comment.D("commenter").D("display_name").As<string>();
            string message = comment.D("message").D("body").As<string>();
            string displayedMessage = $"{displayName}: {message}";
            Line line = new(dateTime, dateTimeEnd, displayedMessage);
            ytt.Lines.Add(line);
        }
        StringWriter stringWriter = new();
        ytt.Save(stringWriter);
        return stringWriter.ToString();
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