using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
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

    public static void GetBackgroundOpacity(this Format format, ref Plicit<long> yttBackgroundOpacity, bool assBackgroundEnable)
    {
        if (format != Format.Ass)
            return;
        yttBackgroundOpacity.Value = assBackgroundEnable ? 254 : 0;
    }

    public static void ApplyFontSize(this Format format, SubtitleDocument sub, long fontSize)
    {
        if (format != Format.Ass)
            return;
        AssDocument ass = (AssDocument)sub;
        foreach (AssStyle style in ass.Styles)
            style.LineHeight = fontSize;
    }
}