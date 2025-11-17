using System;
using System.Collections.Generic;
using System.Drawing;
using YTSubConverter.Shared;
using YTSubConverter.Shared.Formats;
using YTSubConverter.Shared.Formats.Ass;

namespace TwitchChatOffset.Subtitles;

public static class Factories
{
    public static SubtitleDocument NewDocument(this Format format)
        => format switch
        {
            Format.Ytt => new YttDocument(),
            Format.Ass => new AssDocument(new YttDocument()),
            _ => throw new InternalException($"Internal error: non-subtitle format {format} not allowed")
        };

    public static Line NewLine(this Format format, DateTime start, DateTime end, IEnumerable<Section> sections, AnchorPoint anchorPoint, byte windowOpacity)
        => format switch
        {
            Format.Ytt => new Line(start, end, sections)
            {
                AndroidDarkTextHackAllowed = false,
                AnchorPoint = anchorPoint,
                WindowOpacity = windowOpacity
            },
            Format.Ass => NewAssLine(start, end, sections, anchorPoint, windowOpacity),
            _ => throw new InternalException($"Internal error: non-subtitle format {format} not allowed")
        };

    public static AssLine NewAssLine(DateTime start, DateTime end, IEnumerable<Section> sections, AnchorPoint anchorPoint, byte windowOpacity)
    {
        AssLine assLine = new(start, end)
        {
            AndroidDarkTextHackAllowed = false,
            AnchorPoint = anchorPoint,
            Alpha = windowOpacity
        };
        assLine.Sections.AddRange(sections);
        return assLine;
    }

    public static Section NewSection(this Format format, string text)
        => format switch
        {
            Format.Ytt => new Section(text),
            Format.Ass => new AssSection(text),
            _ => throw new InternalException($"Internal error: non-subtitle format {format} not allowed")
        };

    public static Section NewSection(this Format format, string text, Color foreColor)
        => format switch
        {
            Format.Ytt => new Section(text)
            {
                ForeColor = foreColor
            },
            Format.Ass => new AssSection(text)
            {
                ForeColor = foreColor
            },
            _ => throw new InternalException($"Internal error: non-subtitle format {format} not allowed")
        };
}