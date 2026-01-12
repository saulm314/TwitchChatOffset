namespace TwitchChatOffset.Options.Groups;

public static class Aliases
{
    public static AliasesContainer Start { get; } = new(["--start"]);
    public static AliasesContainer End { get; } = new(["--end"]);
    public static AliasesContainer Delay { get; } = new(["--delay"]);
    public static AliasesContainer Format { get; } = new(["--format", "--formatting", "-f"]);
    public static AliasesContainer SubPosition { get; } = new(["--sub-position"]);
    public static AliasesContainer SubMaxMessages { get; } = new(["--sub-max-messages"]);
    public static AliasesContainer SubMaxCharsPerLine { get; } = new(["--sub-max-chars-per-line", "--sub-max-chars"]);
    public static AliasesContainer SubScale { get; } = new(["--sub-scale"]);
    public static AliasesContainer SubFontSize { get; } = new(["--sub-font-size"]);
    public static AliasesContainer SubShadow { get; } = new(["--sub-shadow"]);
    public static AliasesContainer SubOutlineDisable { get; } = new(["--sub-outline-disable"]);
    public static AliasesContainer SubWindowOpacity { get; } = new(["--sub-window-opacity"]);
    public static AliasesContainer SubBackgroundOpacity { get; } = new(["--sub-background-opacity", "--sub-bg-opacity"]);
    public static AliasesContainer SubBackgroundEnable { get; } = new(["--sub-background-enable", "--sub-bg-enable"]);
    public static AliasesContainer SubTextColor { get; } = new(["--sub-text-color"]);
    public static AliasesContainer SubShadowColor { get; } = new(["--sub-shadow-color"]);
    public static AliasesContainer SubBackgroundColor { get; } = new(["--sub-background-color", "--sub-bg-color"]);
    public static AliasesContainer SubColorSeed { get; } = new(["--sub-color-seed"]);
    public static AliasesContainer InputFile { get; } = new(["--input-file"]);
    public static AliasesContainer InputDir { get; } = new(["--input-directory", "--input-dir", "--input", "-i"]);
    public static AliasesContainer OutputFile { get; } = new(["--output-file"]);
    public static AliasesContainer OutputDir { get; } = new(["--output-directory", "--output-dir", "--output", "-o"]);
    public static AliasesContainer Suffix { get; } = new(["--suffix"]);
    public static AliasesContainer Quiet { get; } = new(["--quiet", "-q"]);
    public static AliasesContainer SearchPattern { get; } = new(["--search-pattern", "--pattern"]);
    public static AliasesContainer Response { get; } = new(["--response"]);
    public static AliasesContainer MultiResponse { get; } = new(["--response"]);
    public static AliasesContainer CsvPath { get; } = new(["--csv-path", "--csv"]);
}