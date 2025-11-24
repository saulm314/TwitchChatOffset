using YTSubConverter.Shared;
using static TwitchChatOffset.Options.Groups.CliOptions;

namespace TwitchChatOffset.Options.Groups;

public record SubtitleOptions : OptionGroup<SubtitleOptions>
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

    [CliOption(nameof(CliOptions.SubFontSize))]
    public Plicit<long> SubFontSize;

    [CliOption(nameof(CliOptions.SubBackgroundEnable))]
    public Plicit<bool> SubBackgroundEnable;

    [CliOption(nameof(CliOptions.SubOutlineDisable))]
    public Plicit<bool> SubOutlineDisable;

    public SubtitleSectionOptions SectionOptions = new();
}