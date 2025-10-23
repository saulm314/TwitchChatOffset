using TwitchChatOffset.Ytt;
using YTSubConverter.Shared;

namespace TwitchChatOffset.Csv;

public record TransformManyToManyCsv
(
    string InputFile,
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
    long YttWindowOpacity,
    long YttBackgroundOpacity,
    string YttTextColor,
    string YttShadowColor,
    string YttBackgroundColor,
    string InputDir,
    string OutputDir
);