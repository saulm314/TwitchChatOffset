using TwitchChatOffset.Ytt;
using YTSubConverter.Shared;
using static TwitchChatOffset.Options.Groups.CliOptions;

namespace TwitchChatOffset.Options.Groups;

public struct SubtitleOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(SubtitleOptions));
    private static FieldData[]? _fieldDatas;

    [CliOption(nameof(SubPosition))]
    public Plicit<AnchorPoint> Position;

    [CliOption(nameof(SubMaxMessages))]
    public Plicit<long> MaxMessages;

    [CliOption(nameof(SubMaxCharsPerLine))]
    public Plicit<long> MaxCharsPerLine;

    [CliOption(nameof(SubScale))]
    public Plicit<double> Scale;

    [CliOption(nameof(SubShadow))]
    public Plicit<Shadow> Shadow;

    [CliOption(nameof(SubWindowOpacity))]
    public Plicit<long> WindowOpacity;

    [CliOption(nameof(SubBackgroundOpacity))]
    public Plicit<long> BackgroundOpacity;

    [CliOption(nameof(SubTextColor))]
    public Plicit<string> TextColor;

    [CliOption(nameof(SubShadowColor))]
    public Plicit<string> ShadowColor;

    [CliOption(nameof(SubBackgroundColor))]
    public Plicit<string> BackgroundColor;
}