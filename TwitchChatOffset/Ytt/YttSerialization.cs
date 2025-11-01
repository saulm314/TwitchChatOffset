using TwitchChatOffset.Json;
using TwitchChatOffset.Options.Groups;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using YTSubConverter.Shared;
using YTSubConverter.Shared.Formats;
using static System.Drawing.ColorTranslator;

namespace TwitchChatOffset.Ytt;

public static class YttSerialization
{
    public static string Serialize(JToken json, SubtitleOptions options)
    {
        if (options.MaxMessages < 1)
        {
            PrintWarning("Warning: ytt-max-messages is less than 1 which is not supported; treating it as 1 instead");
            options.MaxMessages.Value = 1;
        }
        if (options.WindowOpacity == 255)
        {
            PrintWarning("Warning: ytt-window-opacity is 255 which for some reason may get overridden in the YouTube player, treatingit as 254 instead");
            options.WindowOpacity.Value = 254;
        }
        if (options.SectionOptions.BackgroundOpacity == 255)
        {
            PrintWarning("Warning: ytt-background-opacity is 255 which for some reason may get overridden in the YouTube player, treating it as 254 instead");
            options.SectionOptions.BackgroundOpacity.Value = 254;
        }
        YttDocument ytt = new();
        JArray comments = json.D("comments").As<JArray>();
        Dictionary<string, Color> userColors = [];
        Queue<ChatMessage> visibleMessages = new((int)options.MaxMessages);
        foreach (JToken comment in comments)
        {
            ChatMessage chatMessage = GetChatMessage(comment, userColors, options);

            if (visibleMessages.Count > 0)
            {
                Line line = GetLine(visibleMessages, chatMessage, options);
                ytt.Lines.Add(line);
            }

            if (visibleMessages.Count >= options.MaxMessages)
                _ = visibleMessages.Dequeue();
            visibleMessages.Enqueue(chatMessage);
        }
        if (comments.Count > 0)
        {
            Line lastLine = GetLine(visibleMessages, null, options);
            ytt.Lines.Add(lastLine);
        }

        StringWriter stringWriter = new();
        ytt.Save(stringWriter);
        return stringWriter.ToString();
    }

    private static ChatMessage GetChatMessage(JToken comment, Dictionary<string, Color> userColors, SubtitleOptions options)
    {
        long offset = comment.D("content_offset_seconds").As<long>();
        TimeSpan timeSpan = TimeSpan.FromSeconds(offset);

        JToken message = comment.D("message");
        string displayName = comment.D("commenter").D("display_name").As<string>();
        string messageStr = message.D("body").As<string>();
        GetWrappedMessage(displayName, messageStr, (int)options.MaxCharsPerLine, out string wrappedDisplayName, out string wrappedMessage);

        Color userColor = GetUserColor(userColors, displayName, message);

        Section displayNameSection = new(wrappedDisplayName)
        {
            ForeColor = userColor
        };
        Section messageSection = new(wrappedMessage)
        {
            ForeColor = FromHtml(options.TextColor)
        };
        displayNameSection.ApplyOptions(options.SectionOptions);
        messageSection.ApplyOptions(options.SectionOptions);

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
        color = FromHtml(userColorStr);
        userColors.Add(user, color);
        return color;
    }

    private static void GetWrappedMessage(string displayName, string message, int maxCharsPerLine, out string wrappedDisplayName, out string wrappedMessage)
    {
        string total = displayName + ": " + message;
        string wrappedTotal = GetWrappedText(total.AsSpan(), maxCharsPerLine);
        int colonIndex = wrappedTotal.IndexOf(':');
        wrappedDisplayName = wrappedTotal[..(colonIndex + 2)];
        wrappedMessage = wrappedTotal[(colonIndex + 2)..];
    }

    private static string GetWrappedText(ReadOnlySpan<char> text, int maxCharsPerLine)
    {
        StringBuilder builder = new();
        int index = 0;
        while (text.Length - index > maxCharsPerLine)
        {
            int whiteSpaceIndex = FindLastIndex(text, index, maxCharsPerLine, char.IsWhiteSpace);
            if (whiteSpaceIndex == -1)
            {
                builder.Append(text[index..(index += maxCharsPerLine)]);
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

    private static Line GetLine(Queue<ChatMessage> chatMessages, ChatMessage? nextChatMessage, SubtitleOptions options)
    {
        List<Section> sections = new(chatMessages.Count * 3);
        ChatMessage lastChatMessage = default;
        foreach (ChatMessage chatMessage in chatMessages)
        {
            sections.Add(chatMessage.Name);
            sections.Add(chatMessage.Message);
            Section newlineSection = new("\n");
            newlineSection.ApplyOptions(options.SectionOptions);
            sections.Add(newlineSection);
            lastChatMessage = chatMessage;
        }
        DateTime start = SubtitleDocument.TimeBase + lastChatMessage.Time;
        DateTime end = SubtitleDocument.TimeBase + (nextChatMessage?.Time ?? TimeSpan.FromHours(12)); // 12 hours is the max YouTube video length
        // if any line has the same start and end value, YTSubConverter appears to automatically get rid of that line when saving the YTT file
        Line line = new(start, end, sections)
        {
            AnchorPoint = options.Position,
            AndroidDarkTextHackAllowed = false,
            WindowOpacity = (byte)options.WindowOpacity
        };
        return line;
    }
}