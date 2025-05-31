namespace TwitchChatOffset;

public readonly record struct Transform(string InputPath, string OutputPath, long Start, long End, Format PFormat);
public readonly record struct TransformManyToMany(string CsvPath, string OutputDir, Format PFormat, bool Quiet);
public readonly record struct TransformOneToMany(string InputPath, string CsvPath, string OutputDir, Format PFormat, bool Quiet);
public readonly record struct TransformAll(string Suffix, string InputDir, string SearchPattern, string OutputDir, Format PFormat, bool Quiet,
    long Start, long End);