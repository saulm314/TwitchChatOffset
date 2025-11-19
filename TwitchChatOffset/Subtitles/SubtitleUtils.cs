using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
using System;
using System.Drawing;
using YTSubConverter.Shared;
using YTSubConverter.Shared.Formats;
using YTSubConverter.Shared.Formats.Ass;

namespace TwitchChatOffset.Subtitles;

public static class SubtitleUtils
{
    public static void ApplyOptions(this Section section, SubtitleSectionOptions options)
    {
        var (scale, shadow, backgroundOpacity, shadowColor, backgroundColor) = options;
        section.Scale = scale;
        section.Offset = OffsetType.Superscript;
        section.BackColor = Color.FromArgb(backgroundOpacity, backgroundColor);
        if (shadow != null)
            section.ShadowColors[(ShadowType)shadow] = shadowColor;
    }

    public static ShadowType? Convert(this Shadow shadow)
        => shadow switch
        {
            Shadow.None => null,
            Shadow.Glow => ShadowType.Glow,
            Shadow.Bevel => ShadowType.Bevel,
            Shadow.HardShadow => ShadowType.HardShadow,
            Shadow.SoftShadow => ShadowType.SoftShadow,
            _ => throw new InternalException("Internal error: unrecognised shadow type")
        };

    public static void NormaliseByte(ref Plicit<long> byteValue)
    {
        byteValue.Value = byteValue.Value switch
        {
            < 0 => 0,
            > 255 => 255,
            _ => byteValue
        };
    }

    public static void ApplyAssStyles(this Format format, SubtitleDocument sub, long fontSize, bool backgroundEnable, long windowOpacity)
    {
        if (format != Format.Ass)
            return;
        AssDocument ass = (AssDocument)sub;
        foreach (AssStyle style in ass.Styles)
        {
            style.LineHeight = fontSize;
            style.BackgroundEnable = backgroundEnable;
            style.WindowOpacity = (byte)windowOpacity;
        }
    }

    // 12 hours is the max YouTube video length
    private static readonly TimeSpan _time12Hours = TimeSpan.FromHours(12);
    public static TimeSpan GetMaxTimeSpan(this Format format, ChatMessage lastChatMessage)
        => format switch
        {
            Format.Ytt => lastChatMessage.Time <= _time12Hours ? _time12Hours : lastChatMessage.Time + _time12Hours,
            Format.Ass => lastChatMessage.Time + _time12Hours,
            _ => throw new InternalException($"Internal error: non-subtitle format {format} not allowed")
        };
}