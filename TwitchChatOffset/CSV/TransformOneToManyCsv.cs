namespace TwitchChatOffset.CSV;

public record TransformOneToManyCsv
(
    string OutputFile,
    long Start,
    long End,
    Format PFormat,
    string OutputDir
);