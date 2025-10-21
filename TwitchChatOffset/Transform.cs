using TwitchChatOffset.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YTSubConverter.Shared;
using YTSubConverter.Shared.Formats;

namespace TwitchChatOffset;

public static class Transform
{
    public static string DoTransform(string inputString, long start, long end, Format format, AnchorPoint yttPosition)
    {
        JObject json = JsonUtils.Deserialize(inputString);
        ApplyOffset(json, start, end);
        return Serialize(json, format, yttPosition);
    }

    public static string DoTransform(JToken inputJson, long start, long end, Format format, AnchorPoint yttPosition)
    {
        JToken json = inputJson.DeepClone();
        ApplyOffset(json, start, end);
        return Serialize(json, format, yttPosition);
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

    public static string Serialize(JToken json, Format format, AnchorPoint yttPosition)
    {
        return format switch
        {
            Format.json => SerializeToJson(json),
            Format.jsonindented => SerializeToJsonIndented(json),
            Format.ytt => SerializeToYtt(json, yttPosition),
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

    public static string SerializeToYtt(JToken json, AnchorPoint yttPosition)
    {
        YttDocument ytt = new();
        JArray comments = json.D("comments").As<JArray>();
        Dictionary<string, Color> userColors = [];
        foreach (JToken comment in comments)
        {
            long offset = comment.D("content_offset_seconds").As<long>();
            TimeSpan timeSpan = TimeSpan.FromSeconds(offset);
            DateTime dateTime = SubtitleDocument.TimeBase + timeSpan;
            DateTime dateTimeEnd = dateTime.AddSeconds(100);

            JToken message = comment.D("message");
            string displayName = comment.D("commenter").D("display_name").As<string>();
            string messageStr = message.D("body").As<string>();
            GetWrappedMessage(displayName, messageStr, out string wrappedDisplayName, out string wrappedMessage);

            Color userColor = GetUserColor(userColors, displayName, message);
            Section displayNameSection = new(wrappedDisplayName)
            {
                ForeColor = userColor
            };
            Section messageSection = new(wrappedMessage)
            {
                ForeColor = Color.White
            };
            Line line = new(dateTime, dateTimeEnd, [displayNameSection, messageSection])
            {
                AnchorPoint = yttPosition
            };
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

    private static Color GetUserColor(Dictionary<string, Color> userColors, string user, JToken message)
    {
        if (userColors.TryGetValue(user, out Color color))
            return color;
        string? userColorStr = message.D("user_color").AsN<string>()?.Value;
        if (userColorStr == null)
        {
            Random random = new();
            int r = random.Next(256);
            int g = random.Next(256);
            int b = random.Next(256);
            color = Color.FromArgb(r, g, b);
            userColors.Add(user, color);
            return color;
        }
        color = ColorTranslator.FromHtml(userColorStr);
        userColors.Add(user, color);
        return color;
    }

    private const int WrapCharLimit = 40;
    private static void GetWrappedMessage(string displayName, string message, out string wrappedDisplayName, out string wrappedMessage)
    {
        string total = displayName + ": " + message;
        string wrappedTotal = GetWrappedText(total.AsSpan());
        int colonIndex = wrappedTotal.IndexOf(':');
        wrappedDisplayName = wrappedTotal[..(colonIndex + 2)];
        wrappedMessage = wrappedTotal[(colonIndex + 2)..];
    }

    private static string GetWrappedText(ReadOnlySpan<char> text)
    {
        StringBuilder builder = new();
        int index = 0;
        while (text.Length - index > WrapCharLimit)
        {
            int whiteSpaceIndex = FindLastIndex(text, index, WrapCharLimit, char.IsWhiteSpace);
            if (whiteSpaceIndex == -1)
            {
                builder.Append(text[index..(index += WrapCharLimit)]);
                builder.Append('\n');
                continue;
            }
            builder.Append(text[index..whiteSpaceIndex]);
            builder.Append('\n');
            index = whiteSpaceIndex + 1;
        }
        builder.Append(text[index..]);
        return builder.ToString();
    }

    private static int FindLastIndex<T>(ReadOnlySpan<T> span, int startIndex, int count, Predicate<T> match)
    {
        for (int i = startIndex + count - 1; i >= startIndex; i--)
            if (match(span[i]))
                return i;
        return -1;
    }
}