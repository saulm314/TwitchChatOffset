using System.Drawing;
using YTSubConverter.Shared;

namespace TwitchChatOffset.Ytt;

public static class SectionUtils
{
    public static void ApplyOptions(this Section section, SectionOptions sectionOptions)
    {
        var (scale, shadow, backgroundOpacity) = sectionOptions;
        section.Scale = scale;
        section.Offset = OffsetType.Superscript;
        section.BackColor = Color.FromArgb(backgroundOpacity, Color.Black);
        if (shadow != null)
            section.ShadowColors[(ShadowType)shadow] = Color.Black;
    }
}