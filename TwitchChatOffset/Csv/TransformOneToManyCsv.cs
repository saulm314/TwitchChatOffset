using YTSubConverter.Shared;

namespace TwitchChatOffset.Csv;

public record TransformOneToManyCsv
(
    string OutputFile,
    long Start,
    long End,
    Format Format,
    AnchorPoint YttPosition,
    string OutputDir
);