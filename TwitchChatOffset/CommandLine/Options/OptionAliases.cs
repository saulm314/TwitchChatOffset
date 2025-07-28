namespace TwitchChatOffset.CommandLine.Options;

public static class OptionAliases
{
    public static AliasesContainer Start { get; } = new(["--start"]);
    public static AliasesContainer End { get; } = new(["--end"]);
    public static AliasesContainer PFormat { get; } = new(["--format", "--formatting", "-f"]);
    public static AliasesContainer InputDir { get; } = new(["--input-directory", "--input", "-i"]);
    public static AliasesContainer OutputDir { get; } = new(["--output-directory", "--output", "-o"]);
    public static AliasesContainer Quiet { get; } = new(["--quiet", "-q"]);
    public static AliasesContainer SearchPattern { get; } = new(["--search-pattern", "--pattern"]);
    public static AliasesContainer POptionPriority { get; } = new(["--option-priority", "--priority"]);
}