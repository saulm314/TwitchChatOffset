global using static TwitchChatOffset.CommandLine.Tokens;
using System.CommandLine;
using TwitchChatOffset.CommandLine.Options;
using YTSubConverter.Shared;
using static TwitchChatOffset.CommandLine.Options.OptionAliases;

namespace TwitchChatOffset.CommandLine;

public static class Tokens
{ 
    public static readonly Argument<string> InputArgument = new("input-path")
    {
        HelpName = "Input path (JSON file)"
    };

    public static readonly Argument<string> OutputArgument = new("output-path")
    {
        HelpName = "Output path"
    };

    public static readonly Argument<string> CsvArgument = new("csv-path")
    {
        HelpName = "Path to the CSV file with data to transform"
    };

    public static readonly Argument<string> SuffixArgument = new("suffix")
    {
        HelpName = "Suffix to be apended to all output file names, including the extension"
    };
    
    public static readonly Option<long> StartOption = new("start", Start.Aliases)
    {
        HelpName = "Starting point in seconds before which to dismiss chat messages (optional)",
        DefaultValueFactory = _ => 0
    };

    public static readonly Option<long> EndOption = new("end", End.Aliases)
    {
        HelpName = "Ending point in seconds after which to dismiss chat messages, or any negative number for no ending point (optional)",
        DefaultValueFactory = _ => -1
    };

    public static readonly Option<long> DelayOption = new("delay", Delay.Aliases)
    {
        HelpName = "Delay in seconds to apply to all messages after cutting out unneeded messages (optional)",
        DefaultValueFactory = _ => 0
    };

    public static readonly Option<Format> FormatOption = new("format", OptionAliases.Format.Aliases)
    {
        HelpName = "Format for the output file (optional)",
        DefaultValueFactory = _ => default
    };

    public static readonly Option<AnchorPoint> YttPositionOption = new("ytt-position", YttPosition.Aliases)
    {
        HelpName = "Position on the screen for YTT subtitles (ytt only) (optional)",
        DefaultValueFactory = _ => default
    };

    public static readonly Option<long> YttMaxMessagesOption = new("ytt-max-messages", YttMaxMessages.Aliases)
    {
        HelpName = "Maximum number of messages to display at once for YTT subtitles (ytt only) (must be at least 1) (optional)",
        DefaultValueFactory = _ => 6
    };

    public static readonly Option<string> OutputDirOption = new("output-dir", OutputDir.Aliases)
    {
        HelpName = "Output directory (will create if doesn't exist) (optional)",
        DefaultValueFactory = _ => "."
    };
    
    public static readonly Option<string> InputDirOption = new("input-dir", InputDir.Aliases)
    {
        HelpName = "Input directory (optional)",
        DefaultValueFactory = _ => "."
    };

    public static readonly Option<bool> QuietOption = new("quiet", Quiet.Aliases)
    {
        HelpName = "Do not print a message for intermediate steps such as individual files being written (optional)",
        DefaultValueFactory = _ => false
    };

    public static readonly Option<string> SearchPatternOption = new("search-pattern", SearchPattern.Aliases)
    {
        HelpName = "Filter which files to transform by name; may contain wildcards '*' (zero or more characters) and '?' (exactly one character) (optional)",
        DefaultValueFactory = _ => "*.json"
    };

    public static readonly Option<long> OptionPriorityOption = new("option-priority", OptionAliases.OptionPriority.Aliases)
    {
        HelpName = "Select priority to determine which options should be used when there is a clash between the CLI options and the CSV options (can be any integer; higher priority wins; if priorities are equal, then CSV is prioritised (optional)",
        DefaultValueFactory = _ => 0
    };
}