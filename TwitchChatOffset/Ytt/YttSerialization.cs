using TwitchChatOffset.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using YTSubConverter.Shared;
using YTSubConverter.Shared.Formats;

namespace TwitchChatOffset.Ytt;

public static class YttSerialization
{
    public static string Serialize(JToken json, AnchorPoint yttPosition, long yttMaxMessages)
    {
        if (yttMaxMessages < 1)
        {
            PrintWarning("Warning: ytt-max-messages is less than 1 which is not supported; treating it as 1 instead");
            yttMaxMessages = 1;
        }
        YttDocument ytt = new();
        JArray comments = json.D("comments").As<JArray>();
        Dictionary<string, Color> userColors = [];
        Queue<ChatMessage> visibleMessages = new((int)yttMaxMessages);
        foreach (JToken comment in comments)
        {
            ChatMessage chatMessage = GetChatMessage(comment, userColors);

            if (visibleMessages.Count > 0)
            {
                Line line = GetLine(visibleMessages, chatMessage, yttPosition);
                ytt.Lines.Add(line);
            }

            if (visibleMessages.Count >= yttMaxMessages)
                _ = visibleMessages.Dequeue();
            visibleMessages.Enqueue(chatMessage);
        }
        Line lastLine = GetLine(visibleMessages, null, yttPosition);
        ytt.Lines.Add(lastLine);

        StringWriter stringWriter = new();
        ytt.Save(stringWriter);
        return stringWriter.ToString();
    }

    private static ChatMessage GetChatMessage(JToken comment, Dictionary<string, Color> userColors)
    {
        long offset = comment.D("content_offset_seconds").As<long>();
        TimeSpan timeSpan = TimeSpan.FromSeconds(offset);

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

        return new(displayNameSection, messageSection, timeSpan);
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

    private static Line GetLine(Queue<ChatMessage> chatMessages, ChatMessage? nextChatMessage, AnchorPoint yttPosition)
    {
        List<Section> sections = new(chatMessages.Count * 3);
        ChatMessage lastChatMessage = default;
        foreach (ChatMessage chatMessage in chatMessages)
        {
            sections.Add(chatMessage.Name);
            sections.Add(chatMessage.Message);
            sections.Add(new("\n"));
            lastChatMessage = chatMessage;
        }
        DateTime start = SubtitleDocument.TimeBase + lastChatMessage.Time;
        DateTime end = SubtitleDocument.TimeBase + (nextChatMessage?.Time ?? lastChatMessage.Time + TimeSpan.FromMinutes(15));
        Line line = new(start, end, sections)
        {
            AnchorPoint = yttPosition
        };
        return line;
    }
}