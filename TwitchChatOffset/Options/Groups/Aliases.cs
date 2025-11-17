namespace TwitchChatOffset.Options.Groups;

public static class Aliases
{
    public static AliasesContainer Start { get; } = new(["--start"]);
    public static AliasesContainer End { get; } = new(["--end"]);
    public static AliasesContainer Delay { get; } = new(["--delay"]);
    public static AliasesContainer Format { get; } = new(["--format", "--formatting", "-f"]);
    public static AliasesContainer SubPosition { get; } = new(["--subtitle-position", "--sub-position", "--ytt-position"]);
    public static AliasesContainer SubMaxMessages { get; } = new(["--subtitle-max-messages", "--sub-max-messages", "--ytt-max-messages"]);
    public static AliasesContainer SubMaxCharsPerLine { get; } = new(["--subtitle-max-chars-per-line","--sub-max-chars-per-line","--ytt-max-chars-per-line",
        "--ass-max-chars-per-line", "--subtitle-max-chars", "--sub-max-chars", "--ytt-max-chars", "--ass-max-chars"]);
    public static AliasesContainer SubScale { get; } = new(["--subtitle-scale", "--sub-scale", "--ytt-scale", "--ass-scale"]);
    public static AliasesContainer SubShadow { get; } = new(["--subtitle-shadow", "--sub-shadow", "--ytt-shadow", "--ass-shadow"]);
    public static AliasesContainer SubWindowOpacity { get; } = new(["--subtitle-window-opacity", "--sub-window-opacity", "--ytt-window-opacity",
        "--ass-window-opacity"]);
    public static AliasesContainer SubBackgroundOpacity { get; } = new(["--subtitle-background-opacity", "--sub-background-opacity", "--ytt-background-opacity",
        "--ass-backgrond-opacity", "--subtitle-bg-opacity", "--sub-bg-opacity", "--ytt-bg-opacity", "--ass-bg-opacity"]);
    public static AliasesContainer SubTextColor { get; } = new(["--subtitle-text-color", "--sub-text-color", "--ytt-text-color", "--ass-text-color"]);
    public static AliasesContainer SubShadowColor { get; } = new(["--subtitle-shadow-color", "--sub-shadow-color", "--ytt-shadow-color", "--ass-shadow-color"]);
    public static AliasesContainer SubBackgroundColor { get; } = new(["--subtitle-background-color", "--sub-background-color", "--ytt-background-color",
        "--ass-background-color", "--subtitle-bg-color", "--sub-bg-color", "--ytt-bg-color", "--ass-bg-color"]);
    public static AliasesContainer InputFile { get; } = new(["--input-file"]);
    public static AliasesContainer InputDir { get; } = new(["--input-directory", "--input", "-i"]);
    public static AliasesContainer OutputFile { get; } = new(["--output-file"]);
    public static AliasesContainer OutputDir { get; } = new(["--output-directory", "--output-dir", "--output", "-o"]);
    public static AliasesContainer Suffix { get; } = new(["--suffix"]);
    public static AliasesContainer Quiet { get; } = new(["--quiet", "-q"]);
    public static AliasesContainer SearchPattern { get; } = new(["--search-pattern", "--pattern"]);
    public static AliasesContainer Response { get; } = new(["--response"]);
    public static AliasesContainer MultiResponse { get; } = new(["--response"]);
    public static AliasesContainer CsvPath { get; } = new(["--csv-path", "--csv"]);
}