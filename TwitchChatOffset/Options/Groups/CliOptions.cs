using TwitchChatOffset.Ytt;
using YTSubConverter.Shared;
using static TwitchChatOffset.Options.CliOptionContainer;

namespace TwitchChatOffset.Options.Groups;

public static class CliOptions
{
    public static CliOptionContainer Start { get; } = New<long>("--start", Aliases.Start,
        "Starting point in seconds before which to dismiss chat messages (optional)",
        _ => 0);

    public static CliOptionContainer End { get; } = New<long>("--end", Aliases.End,
        "Ending point in seconds after which to dismiss chat messages, or any negative number for no ending point (optional)",
        _ => -1);

    public static CliOptionContainer Delay { get; } = New<long>("--delay", Aliases.Delay,
        "Delay in seconds to apply to all messages after cutting out unneeded messages (optional)",
        _ => 0);

    public static CliOptionContainer Format { get; } = New("--format", Aliases.Format,
        "Format for the output file (optional)",
        _ => TwitchChatOffset.Format.Json);

    public static CliOptionContainer SubPosition { get; } = New("--subtitle-position", Aliases.SubPosition,
        "Position on the screen for subtitles (optional)",
        _ => AnchorPoint.TopLeft);

    public static CliOptionContainer SubMaxMessages { get; } = New<long>("--subtitle-max-messages", Aliases.SubMaxMessages,
        "Maximum number of messages to display at once for subtitles (must be at least 1) (optional)",
        _ => 4);

    public static CliOptionContainer SubMaxCharsPerLine { get; } = New<long>("--subtitle-max-chars-per-line", Aliases.SubMaxCharsPerLine,
        "Maximum number of characters to display in a single subtitle line before it wraps to a New line (optional)",
        _ => 40);

    public static CliOptionContainer SubScale { get; } = New("--subtitle-scale", Aliases.SubScale,
        "Subtitle size (e.g. 0, 0.5, 1.5, etc.) (must be at least 0) (optional)",
        _ => 0.0);

    public static CliOptionContainer SubShadow { get; } = New("--subtitle-shadow", Aliases.SubShadow,
        "Shadow type (or none) for subtitles (optional)",
        _ => Shadow.Glow);

    public static CliOptionContainer SubWindowOpacity { get; } = New<long>("--subtitle-window-opacity", Aliases.SubWindowOpacity,
        "Window opacity for subtitles (the text box containing the subtitles), ranging from 0 (fully transparent) to 254 (fully opaque) (optional)",
        _ => 0);

    public static CliOptionContainer SubBackgroundOpacity { get; } = New<long>("--subtitle-background-opacity", Aliases.SubBackgroundOpacity,
        "Background opacity for subtitles (the text only, not the entire text box), ranging from 0 (fully transparent) to 254 (fully opaque) (optional)",
        _ => 0);

    public static CliOptionContainer SubTextColor { get; } = New("--subtitle-text-color", Aliases.SubTextColor,
        "Text color for subtitles, (e.g. \"white\", \"#B0B0B0\", etc.) (optional)",
        _ => "white");

    public static CliOptionContainer SubShadowColor { get; } = New("--subtitle-shadow-color", Aliases.SubShadowColor,
        "Shadow color for subtitles, (e.g. \"black\", \"#B0B0B0\", etc.) (optional)",
        _ => "black");

    public static CliOptionContainer SubBackgroundColor { get; } = New("--subtitle-background-color", Aliases.SubBackgroundColor,
        "Background color for subtitles, (e.g. \"black\", \"#B0B0B0\", etc.) (optional)",
        _ => "black");

    public static CliOptionContainer OutputDir { get; } = New("--output-directory", Aliases.OutputDir,
        "Output directory (will create if doesn't exist) (optional)",
        _ => ".");
    
    public static CliOptionContainer InputDir { get; } = New("--input-dir", Aliases.InputDir,
        "Input directory (optional)",
        _ => ".");

    public static CliOptionContainer Quiet { get; } = New("--quiet", Aliases.Quiet,
        "Do not print a message for intermediate steps such as individual files being written (optional)",
        _ => false);

    public static CliOptionContainer SearchPattern { get; } = New("--search-pattern", Aliases.SearchPattern,
        "Filter which files to transform by name; may contain wildcards '*' (zero or more characters) and '?' (exactly one character) (optional)",
        _ => "*.json");
}