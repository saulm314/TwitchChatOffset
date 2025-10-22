using TwitchChatOffset.Ytt;
using YTSubConverter.Shared;

namespace TwitchChatOffset.Csv;

public record TransformOneToManyCsv
(
    string OutputFile,
    long Start,
    long End,
    long Delay,
    Format Format,
    AnchorPoint YttPosition,
    long YttMaxMessages,
    long YttMaxCharsPerLine,
    double YttScale,
    Shadow YttShadow,
    long BackgroundOpacity,
    string TextColor,
    string ShadowColor,
    string BackgroundColor,
    string OutputDir
);