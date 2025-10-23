global using static TwitchChatOffset.CommandLine.Tokens;
using TwitchChatOffset.CommandLine.Options;
using TwitchChatOffset.Ytt;
using System.CommandLine;
using YTSubConverter.Shared;
using static TwitchChatOffset.CommandLine.Options.OptionAliases;

namespace TwitchChatOffset.CommandLine;

public static class Tokens
{ 
    public static readonly Argument<string> InputArgument = new("input-path")
    {
        Description = "Input path (JSON file)"
    };

    public static readonly Argument<string> OutputArgument = new("output-path")
    {
        Description = "Output path"
    };

    public static readonly Argument<string> CsvArgument = new("csv-path")
    {
        Description = "Path to the CSV file with data to transform"
    };

    public static readonly Argument<string> SuffixArgument = new("suffix")
    {
        Description = "Suffix to be appended to all output file names, including the extension (e.g. \".ytt\", \"-cropped.json\")"
    };
    
    public static readonly Option<long> StartOption = new("--start", Start.Aliases)
    {
        Description = "Starting point in seconds before which to dismiss chat messages (optional)",
        DefaultValueFactory = _ => 0
    };

    public static readonly Option<long> EndOption = new("--end", End.Aliases)
    {
        Description = "Ending point in seconds after which to dismiss chat messages, or any negative number for no ending point (optional)",
        DefaultValueFactory = _ => -1
    };

    public static readonly Option<long> DelayOption = new("--delay", Delay.Aliases)
    {
        Description = "Delay in seconds to apply to all messages after cutting out unneeded messages (optional)",
        DefaultValueFactory = _ => 0
    };

    public static readonly Option<Format> FormatOption = new("--format", OptionAliases.Format.Aliases)
    {
        Description = "Format for the output file (optional)",
        DefaultValueFactory = _ => default
    };

    public static readonly Option<AnchorPoint> YttPositionOption = new("--ytt-position", YttPosition.Aliases)
    {
        Description = "Position on the screen for YTT subtitles (ytt only) (optional)",
        DefaultValueFactory = _ => AnchorPoint.TopLeft
    };

    public static readonly Option<long> YttMaxMessagesOption = new("--ytt-max-messages", YttMaxMessages.Aliases)
    {
        Description = "Maximum number of messages to display at once for YTT subtitles (ytt only) (must be at least 1) (optional)",
        DefaultValueFactory = _ => 6
    };

    public static readonly Option<long> YttMaxCharsPerLineOption = new("--ytt-max-chars-per-line", YttMaxCharsPerLine.Aliases)
    {
        Description = "Maximum number of characters to display in a single line before it wraps to a new line (ytt only) (optional)",
        DefaultValueFactory = _ => 55
    };

    public static readonly Option<double> YttScaleOption = new("--ytt-scale", YttScale.Aliases)
    {
        Description = "YTT subtitle size (e.g. 0, 0.5, 1.5, etc.) (ytt only) (must be at least 0) (optional)",
        DefaultValueFactory = _ => 0.0
    };

    public static readonly Option<Shadow> YttShadowOption = new("--ytt-shadow", YttShadow.Aliases)
    {
        Description = "Shadow type (or none) for YTT subtitles (ytt only) (optional)",
        DefaultValueFactory = _ => Shadow.Glow
    };

    public static readonly Option<long> YttBackgroundOpacityOption = new("--ytt-background-opacity", YttBackgroundOpacity.Aliases)
    {
        Description = "Background opacity for YTT subtitles, ranging from 0 (fully transparent) to 254 (fully opaque) (ytt only) (optional)",
        DefaultValueFactory = _ => 0
    };

    public static readonly Option<string> YttTextColorOption = new("--ytt-text-color", YttTextColor.Aliases)
    {
        Description = "Text color for YTT subtitles, (e.g. \"white\", \"#B0B0B0\", etc.) (ytt only) (optional)",
        DefaultValueFactory = _ => "white"
    };

    public static readonly Option<string> YttShadowColorOption = new("--ytt-shadow-color", YttShadowColor.Aliases)
    {
        Description = "Shadow color for YTT subtitles, (e.g. \"black\", \"#B0B0B0\", etc.) (ytt only) (optional)",
        DefaultValueFactory = _ => "black"
    };

    public static readonly Option<string> YttBackgroundColorOption = new("--ytt-background-color", YttBackgroundColor.Aliases)
    {
        Description = "Background color for YTT subtitles, (e.g. \"black\", \"#B0B0B0\", etc.) (ytt only) (optional)",
        DefaultValueFactory = _ => "black"
    };

    public static readonly Option<string> OutputDirOption = new("--output-directory", OutputDir.Aliases)
    {
        Description = "Output directory (will create if doesn't exist) (optional)",
        DefaultValueFactory = _ => "."
    };
    
    public static readonly Option<string> InputDirOption = new("--input-dir", InputDir.Aliases)
    {
        Description = "Input directory (optional)",
        DefaultValueFactory = _ => "."
    };

    public static readonly Option<bool> QuietOption = new("--quiet", Quiet.Aliases)
    {
        Description = "Do not print a message for intermediate steps such as individual files being written (optional)",
        DefaultValueFactory = _ => false
    };

    public static readonly Option<string> SearchPatternOption = new("--search-pattern", SearchPattern.Aliases)
    {
        Description = "Filter which files to transform by name; may contain wildcards '*' (zero or more characters) and '?' (exactly one character) (optional)",
        DefaultValueFactory = _ => "*.json"
    };

    public static readonly Option<long> OptionPriorityOption = new("--option-priority", OptionAliases.OptionPriority.Aliases)
    {
        Description = "Select priority to determine which options should be used when there is a clash between the CLI options and the CSV options (can be any integer; higher priority wins; if priorities are equal, then CSV is prioritised (optional)",
        DefaultValueFactory = _ => 0
    };
}