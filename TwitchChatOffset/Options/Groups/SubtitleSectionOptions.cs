using TwitchChatOffset.Ytt;
using System.Drawing;
using YTSubConverter.Shared;
using static TwitchChatOffset.Options.Groups.CliOptions;
using static System.Drawing.ColorTranslator;

namespace TwitchChatOffset.Options.Groups;

public record SubtitleSectionOptions : IOptionGroup<SubtitleSectionOptions>
{
    [CliOption(nameof(SubScale))]
    public Plicit<double> Scale;

    [CliOption(nameof(SubShadow))]
    public Plicit<Shadow> Shadow;

    [CliOption(nameof(SubBackgroundOpacity))]
    public Plicit<long> BackgroundOpacity;

    [CliOption(nameof(SubShadowColor))]
    public Plicit<string> ShadowColor;

    [CliOption(nameof(SubBackgroundColor))]
    public Plicit<string> BackgroundColor;

    public void Deconstruct(out float scale, out ShadowType? shadow, out byte backgroundOpacity, out Color shadowColor, out Color backgroundColor)
    {
        scale = (float)Scale;
        shadow = Shadow.Value.Convert();
        backgroundOpacity = (byte)BackgroundOpacity;
        shadowColor = FromHtml(ShadowColor);
        backgroundColor = FromHtml(BackgroundColor);
    }
}