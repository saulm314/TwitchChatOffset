using TwitchChatOffset.Subtitles;
using System.Drawing;
using YTSubConverter.Shared;
using static TwitchChatOffset.Options.Groups.CliOptions;
using static System.Drawing.ColorTranslator;

namespace TwitchChatOffset.Options.Groups;

public record SubtitleSectionOptions : IOptionGroup<SubtitleSectionOptions>
{
    [CliOption(nameof(CliOptions.YttScale))]
    public Plicit<double> YttScale;

    [CliOption(nameof(SubShadow))]
    public Plicit<Shadow> Shadow;

    [CliOption(nameof(CliOptions.YttBackgroundOpacity))]
    public Plicit<long> YttBackgroundOpacity;

    [CliOption(nameof(SubShadowColor))]
    public Plicit<string> ShadowColor;

    [CliOption(nameof(CliOptions.YttBackgroundColor))]
    public Plicit<string> YttBackgroundColor;

    public void Deconstruct(out float yttScale, out ShadowType? shadow, out byte yttBackgroundOpacity, out Color shadowColor, out Color yttBackgroundColor)
    {
        yttScale = (float)YttScale;
        shadow = Shadow.Value.Convert();
        yttBackgroundOpacity = (byte)YttBackgroundOpacity;
        shadowColor = FromHtml(ShadowColor);
        yttBackgroundColor = FromHtml(YttBackgroundColor);
    }
}