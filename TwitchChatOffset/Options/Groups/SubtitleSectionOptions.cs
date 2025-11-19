using TwitchChatOffset.Subtitles;
using System.Drawing;
using YTSubConverter.Shared;
using static TwitchChatOffset.Options.Groups.CliOptions;
using static System.Drawing.ColorTranslator;

namespace TwitchChatOffset.Options.Groups;

public record SubtitleSectionOptions : IOptionGroup<SubtitleSectionOptions>
{
    [CliOption(nameof(CliOptions.SubScale))]
    public Plicit<double> SubScale;

    [CliOption(nameof(SubShadow))]
    public Plicit<Shadow> Shadow;

    [CliOption(nameof(CliOptions.SubBackgroundOpacity))]
    public Plicit<long> SubBackgroundOpacity;

    [CliOption(nameof(SubShadowColor))]
    public Plicit<string> ShadowColor;

    [CliOption(nameof(CliOptions.SubBackgroundColor))]
    public Plicit<string> SubBackgroundColor;

    public void Deconstruct(out float scale, out ShadowType? shadow, out byte backgroundOpacity, out Color shadowColor, out Color backgroundColor)
    {
        scale = (float)SubScale;
        shadow = Shadow.Value.Convert();
        backgroundOpacity = (byte)SubBackgroundOpacity;
        shadowColor = FromHtml(ShadowColor);
        backgroundColor = FromHtml(SubBackgroundColor);
    }
}