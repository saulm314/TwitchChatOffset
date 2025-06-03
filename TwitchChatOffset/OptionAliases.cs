namespace TwitchChatOffset;

public static class OptionAliases
{
    public static string[] Start { get; } = ["--start"];
    public static string[] End { get; } = ["--end"];
    public static string[] PFormat { get; } = ["--format", "--formatting", "-f"];
    public static string[] InputDir { get; } = ["--input-directory", "--input", "-i"];
    public static string[] OutputDir { get; } = ["--output-directory", "--output", "-o"];
    public static string[] Quiet { get; } = ["--quiet", "-q"];
    public static string[] SearchPattern { get; } = ["--search-pattern"];
}