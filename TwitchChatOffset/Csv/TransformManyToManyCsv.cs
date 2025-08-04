namespace TwitchChatOffset.Csv;

public record TransformManyToManyCsv
(
    string InputFile,
    string OutputFile,
    long Start,
    long End,
    Format PFormat,
    string OutputDir
);