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
    string OutputDir
);