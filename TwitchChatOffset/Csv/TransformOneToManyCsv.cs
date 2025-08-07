namespace TwitchChatOffset.Csv;

public record TransformOneToManyCsv
(
    string OutputFile,
    long Start,
    long End,
    Format Format,
    string OutputDir
);