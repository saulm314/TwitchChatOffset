namespace TwitchChatOffset.CommandLine.Options;

public static class OptionAliases
{
    public static AliasesContainer Start { get; } = new(["--start"]);
    public static AliasesContainer End { get; } = new(["--end"]);
    public static AliasesContainer Delay { get; } = new(["--delay"]);
    public static AliasesContainer Format { get; } = new(["--format", "--formatting", "-f"]);
    public static AliasesContainer YttPosition { get; } = new(["--ytt-position"]);
    public static AliasesContainer YttMaxMessages { get; } = new(["--ytt-max-messages"]);
    public static AliasesContainer YttMaxCharsPerLine { get; } = new(["--ytt-max-chars-per-line", "--ytt-max-chars"]);
    public static AliasesContainer YttScale { get; } = new(["--ytt-scale"]);
    public static AliasesContainer YttShadow { get; } = new(["--ytt-shadow"]);
    public static AliasesContainer YttWindowOpacity { get; } = new(["--ytt-window-opacity"]);
    public static AliasesContainer YttBackgroundOpacity { get; } = new(["--ytt-background-opacity", "--ytt-bg-opacity"]);
    public static AliasesContainer YttTextColor { get; } = new(["--ytt-text-color"]);
    public static AliasesContainer YttShadowColor { get; } = new(["--ytt-shadow-color"]);
    public static AliasesContainer YttBackgroundColor { get; } = new(["--ytt-background-color", "--ytt-bg-color"]);
    public static AliasesContainer InputDir { get; } = new(["--input-directory", "--input", "-i"]);
    public static AliasesContainer OutputDir { get; } = new(["--output-directory", "--output-dir", "--output", "-o"]);
    public static AliasesContainer Quiet { get; } = new(["--quiet", "-q"]);
    public static AliasesContainer SearchPattern { get; } = new(["--search-pattern", "--pattern"]);
    public static AliasesContainer OptionPriority { get; } = new(["--option-priority", "--priority"]);
}