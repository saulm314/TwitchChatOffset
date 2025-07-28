using TwitchChatOffset.CommandLine.Arguments;
using TwitchChatOffset.CommandLine.Options;
using static TwitchChatOffset.CommandLine.Options.OptionAliases;

namespace TwitchChatOffset.CommandLine;

public class Tokens
{
    public TCOArgument<string> InputArgument => inputArgument ??= new("input-path", "Input path (JSON file)");
    public TCOArgument<string> OutputArgument => outputArgument ??= new("output-path", "Output path");
    public TCOArgument<string> CsvArgument => csvArgument ??= new("csv-path", "Path to the CSV file with data to transform");
    public TCOArgument<string> SuffixArgument => suffixArgument ??= new("suffix", "Suffix to be apended to all output file names, including the extension");
    
    public NullableOption<long> StartOption => startOption ??= NullableOption<long>.New(Start.aliases, () => 0, "Starting point in seconds before which to dismiss chat messages (optional)");
    public NullableOption<long> EndOption => endOption ??= NullableOption<long>.New(End.aliases, () => -1, "Ending point in seconds after which to dismiss chat messages, or any negative number for no ending point (optional)");
    public NullableOption<Format> FormatOption => formatOption ??= NullableOption<Format>.New(PFormat.aliases, () => default, "Format for the output file (optional)");
    public NullableOption<string> OutputDirOption => outputDirOption ??= NullableOption<string>.New(OutputDir.aliases, () => ".", "Output directory (will create if doesn't exist) (optional)");
    
    public TCOOption<string> InputDirOption => inputDirOption ??= new(InputDir.aliases, () => ".", "Input directory (optional)");
    public TCOOption<bool> QuietOption => quietOption ??= new(Quiet.aliases, () => false, "Do not print a message after each file is written (optional)");
    public TCOOption<string> SearchPatternOption => searchPatternOption ??= new(SearchPattern.aliases, () => "*.json", "Filter which files to transform by name; may contain wildcards '*' (zero or more characters) and '?' (exactly one character) (optional)");
    public TCOOption<long> OptionPriorityOption => optionPriorityOption ??= new(POptionPriority.aliases, () => 0, "Select priority to determine which options should be used when there is a clash between the CLI options and the CSV options (can be any integer; higher priority wins; if priorities are equal, then CSV is prioritised (optional)");

    private TCOArgument<string>? inputArgument;
    private TCOArgument<string>? outputArgument;
    private TCOArgument<string>? csvArgument;
    private TCOArgument<string>? suffixArgument;

    private NullableOption<long>? startOption;
    private NullableOption<long>? endOption;
    private NullableOption<Format>? formatOption;
    private NullableOption<string>? outputDirOption;

    private TCOOption<string>? inputDirOption;
    private TCOOption<bool>? quietOption;
    private TCOOption<string>? searchPatternOption;
    private TCOOption<long>? optionPriorityOption;
}