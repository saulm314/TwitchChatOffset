using TwitchChatOffset.ConsoleUtils;
using TwitchChatOffset.Subtitles;
using YTSubConverter.Shared;

namespace TwitchChatOffset.Options.Groups;

public static class CliOptions
{
    public static CliOptionContainer<long> Start { get; } = new("--start", Aliases.Start,
        "Starting point in seconds before which to dismiss chat messages (optional)",
        _ => 0);

    public static CliOptionContainer<long> End { get; } = new("--end", Aliases.End,
        "Ending point in seconds after which to dismiss chat messages, or any negative number for no ending point (optional)",
        _ => -1);

    public static CliOptionContainer<long> Delay { get; } = new("--delay", Aliases.Delay,
        "Delay in seconds to apply to all messages after cutting out unneeded messages (optional)",
        _ => 0);

    public static CliOptionContainer<Format> Format { get; } = new("--format", Aliases.Format,
        "Format for the output file (optional)",
        _ => TwitchChatOffset.Format.Json);

    public static CliOptionContainer<AnchorPoint> SubPosition { get; } = new("--subtitle-position", Aliases.SubPosition,
        "Position on the screen for subtitles (optional)",
        _ => AnchorPoint.TopLeft);

    public static CliOptionContainer<long> SubMaxMessages { get; } = new("--subtitle-max-messages", Aliases.SubMaxMessages,
        "Maximum number of messages to display at once for subtitles (must be at least 1) (optional)",
        _ => 4);

    public static CliOptionContainer<long> SubMaxCharsPerLine { get; } = new("--subtitle-max-chars-per-line", Aliases.SubMaxCharsPerLine,
        "Maximum number of characters to display in a single subtitle line before it wraps to a New line (optional)",
        _ => 40);

    public static CliOptionContainer<double> YttScale { get; } = new("--ytt-scale", Aliases.YttScale,
        "YTT subtitle size (e.g. 0, 0.5, 1.5, etc.) (must be at least 0) (optional)",
        _ => 0.0);

    public static CliOptionContainer<long> AssFontSize { get; } = new("--ass-font-size", Aliases.AssFontSize,
        "ASS subtitle font size (e.g. 18, 15, 25, etc.) (must be a positive integer) (optional)",
        _ => 18);

    public static CliOptionContainer<Shadow> SubShadow { get; } = new("--subtitle-shadow", Aliases.SubShadow,
        "Shadow type (or none) for subtitles (optional)",
        _ => Shadow.Glow);

    public static CliOptionContainer<long> SubWindowOpacity { get; } = new("--subtitle-window-opacity", Aliases.SubWindowOpacity,
        "Window opacity for subtitles (the text box containing the subtitles), ranging from 0 (fully transparent) to 255 (fully opaque) (optional)",
        _ => 0);

    public static CliOptionContainer<long> YttBackgroundOpacity { get; } = new("--ytt-background-opacity", Aliases.YttBackgroundOpacity,
        "Background opacity for YTT subtitles (the text only, not the entire text box), ranging from 0 (fully transparent) to 254 (fully opaque) (optional)",
        _ => 0);

    public static CliOptionContainer<bool> AssBackgroundEnable { get; } = new("--ass-background-enable", Aliases.AssBackgroundEnable,
        "Enable black opaque background for ASS subtitles (the text only, not the entire text box, overrides sub-window-opacity to 0) (optional)",
        _ => false);

    public static CliOptionContainer<string> SubTextColor { get; } = new("--subtitle-text-color", Aliases.SubTextColor,
        "Text color for subtitles, (e.g. \"white\", \"#B0B0B0\", etc.) (optional)",
        _ => "white");

    public static CliOptionContainer<string> SubShadowColor { get; } = new("--subtitle-shadow-color", Aliases.SubShadowColor,
        "Shadow color for subtitles, (e.g. \"black\", \"#B0B0B0\", etc.) (optional)",
        _ => "black");

    public static CliOptionContainer<string> YttBackgroundColor { get; } = new("--ytt-background-color", Aliases.YttBackgroundColor,
        "Background color for YTT subtitles, (e.g. \"black\", \"#B0B0B0\", etc.) (optional)",
        _ => "black");

    public static CliOptionContainer<string> InputFile { get; } = new("--input-file", Aliases.InputFile,
        "Input file (optional)",
        _ => string.Empty);

    public static CliOptionContainer<string> InputDir { get; } = new("--input-dir", Aliases.InputDir,
        "Input directory (optional)",
        _ => ".");

    public static CliOptionContainer<string> OutputFile { get; } = new("--output-file", Aliases.OutputFile,
        "Output file (optional)",
        _ => string.Empty);

    public static CliOptionContainer<string> OutputDir { get; } = new("--output-directory", Aliases.OutputDir,
        "Output directory (will create if doesn't exist) (optional)",
        _ => ".");

    public static CliOptionContainer<string> Suffix { get; } = new("--suffix", Aliases.Suffix,
        "Suffix override for output file, e.g. \".json\", \".ytt\", \"-transformed.json\", etc., or \"/auto\" to keep the same as input file (optional)",
        _ => "/auto");

    public static CliOptionContainer<bool> Quiet { get; } = new("--quiet", Aliases.Quiet,
        "Do not print a message for intermediate steps such as individual files being written (optional)",
        _ => false);

    public static CliOptionContainer<string> SearchPattern { get; } = new("--search-pattern", Aliases.SearchPattern,
        "Filter which files to transform by name; may contain wildcards '*' (zero or more characters) and '?' (exactly one character) (optional)",
        _ => "*.json");

    public static CliOptionContainer<CliResponse> Response { get; } = new("--response", Aliases.Response,
        "Set automatic response to any prompts (optional)",
        _ => CliResponse.Manual);

    public static CliOptionContainer<CliMultiResponse> MultiResponse { get; } = new("--response", Aliases.MultiResponse,
        "Set automatic response to any prompts (optional)",
        _ => CliMultiResponse.Manual);

    public static CliOptionContainer<string> CsvPath { get; } = new("--csv-path", Aliases.CsvPath,
        "CSV path to specify options for multiple outputs per input (optional)",
        _ => string.Empty);
}