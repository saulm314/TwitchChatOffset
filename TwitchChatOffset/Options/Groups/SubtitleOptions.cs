using YTSubConverter.Shared;
using static TwitchChatOffset.Options.Groups.CliOptions;

namespace TwitchChatOffset.Options.Groups;

public record SubtitleOptions : IOptionGroup<SubtitleOptions>
{
    [CliOption(nameof(SubPosition))]
    public Plicit<AnchorPoint> Position;

    [CliOption(nameof(SubMaxMessages))]
    public Plicit<long> MaxMessages;

    [CliOption(nameof(SubMaxCharsPerLine))]
    public Plicit<long> MaxCharsPerLine;

    [CliOption(nameof(SubWindowOpacity))]
    public Plicit<long> WindowOpacity;

    [CliOption(nameof(SubTextColor))]
    public Plicit<string> TextColor;

    [CliOption(nameof(CliOptions.AssFontSize))]
    public Plicit<long> AssFontSize;

    [CliOption(nameof(CliOptions.AssBackgroundEnable))]
    public Plicit<bool> AssBackgroundEnable;

    public SubtitleSectionOptions SectionOptions = new();
}