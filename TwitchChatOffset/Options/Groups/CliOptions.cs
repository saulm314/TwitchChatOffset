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

    public static CliOptionContainer<AnchorPoint> SubPosition { get; } = new("--sub-position", Aliases.SubPosition,
        "[YTT|ASS] Position on the screen for subtitles (optional)",
        _ => AnchorPoint.TopLeft);

    public static CliOptionContainer<long> SubMaxMessages { get; } = new("--sub-max-messages", Aliases.SubMaxMessages,
        "[YTT|ASS] Maximum number of messages to display at once for subtitles (must be at least 1) (optional)",
        _ => 4);

    public static CliOptionContainer<long> SubMaxCharsPerLine { get; } = new("--sub-max-chars-per-line", Aliases.SubMaxCharsPerLine,
        "[YTT|ASS] Maximum number of characters to display in a single subtitle line before it wraps to a New line (optional)",
        _ => 40);

    public static CliOptionContainer<double> SubScale { get; } = new("--sub-scale", Aliases.SubScale,
        "[YTT] YTT subtitle size (e.g. 0, 0.5, 1.5, etc.) (must be at least 0) (YTT only) (optional)",
        _ => 0.0);

    public static CliOptionContainer<long> SubFontSize { get; } = new("--sub-font-size", Aliases.SubFontSize,
        "[ASS] ASS subtitle font size (e.g. 18, 15, 25, etc.) (must be a positive integer) (ASS only) (optional)",
        _ => 18);

    public static CliOptionContainer<Shadow> SubShadow { get; } = new("--sub-shadow", Aliases.SubShadow,
        "[YTT] YTT Shadow type (or none) for subtitles (YTT only) (optional)",
        _ => Shadow.Glow);

    public static CliOptionContainer<bool> SubOutlineDisable { get; } = new("--sub-outline-disable", Aliases.SubOutlineDisable,
        "[ASS] Disable outline for ASS subtitles (ASS only) (optional)",
        _ => false);

    public static CliOptionContainer<long> SubWindowOpacity { get; } = new("--sub-window-opacity", Aliases.SubWindowOpacity,
        "[YTT|ASS] Window opacity for subtitles (the text box containing the subtitles), ranging from 0 (fully transparent) to 255 (fully opaque) (optional)",
        _ => 0);

    public static CliOptionContainer<long> SubBackgroundOpacity { get; } = new("--sub-background-opacity", Aliases.SubBackgroundOpacity,
        "[YTT] Background opacity for YTT subtitles (the text only, not the entire text box), ranging from 0 (fully transparent) to 254 (fully opaque) (YTT only) (optional)",
        _ => 0);

    public static CliOptionContainer<bool> SubBackgroundEnable { get; } = new("--sub-background-enable", Aliases.SubBackgroundEnable,
        "[ASS] Enable black opaque background for ASS subtitles (the text only, not the entire text box, overrides sub-window-opacity to 0) (ASS only) (optional)",
        _ => false);

    public static CliOptionContainer<string> SubTextColor { get; } = new("--sub-text-color", Aliases.SubTextColor,
        "[YTT|ASS] Text color for subtitles, (e.g. \"white\", \"#B0B0B0\", etc.) (optional)",
        _ => "white");

    public static CliOptionContainer<string> SubShadowColor { get; } = new("--sub-shadow-color", Aliases.SubShadowColor,
        "[YTT|ASS] Shadow color for subtitles, (e.g. \"black\", \"#B0B0B0\", etc.) (optional)",
        _ => "black");

    public static CliOptionContainer<string> SubBackgroundColor { get; } = new("--sub-background-color", Aliases.SubBackgroundColor,
        "[YTT] Background color for YTT subtitles, (e.g. \"black\", \"#B0B0B0\", etc.) (YTT only) (optional)",
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