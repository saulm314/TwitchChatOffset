using System.Drawing;
using YTSubConverter.Shared;

namespace TwitchChatOffset.Ytt;

public static class YttUtils
{
    public static void ApplyOptions(this Section section, SectionOptions sectionOptions)
    {
        var (scale, shadow, backgroundOpacity, shadowColor, backgroundColor) = sectionOptions;
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
}