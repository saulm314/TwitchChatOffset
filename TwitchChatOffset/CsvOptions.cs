namespace TwitchChatOffset;

public readonly record struct CsvOptions(long Start, long End, Format PFormat, string OutputDir);