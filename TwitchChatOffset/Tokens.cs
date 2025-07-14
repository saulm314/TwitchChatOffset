using System.CommandLine;
using static TwitchChatOffset.OptionAliases;

namespace TwitchChatOffset;

public class Tokens
{
    public Argument<string> InputArgument => inputArgument ??= new("input-path", "Input path (JSON file)");
    public Argument<string> OutputArgument => outputArgument ??= new("output-path", "Output path");
    public Argument<string> CsvArgument => csvArgument ??= new("csv-path", "Path to the CSV file with data to transform");
    public Argument<string> SuffixArgument => suffixArgument ??= new("suffix", "Suffix to be apended to all output file names, including the extension");
    public Option<long> StartOption => startOption ??= new(Start.aliases, () => 0, "Starting point in seconds before which to dismiss chat messages (optional)");
    public Option<long> EndOption => endOption ??= new(End.aliases, () => -1, "Ending point in seconds after which to dismiss chat messages (optional)");
    public Option<Format> FormatOption => formatOption ??= new(PFormat.aliases, () => default, "Format for the output file (optional)");
    public Option<string> InputDirOption => inputDirOption ??= new(InputDir.aliases, () => ".", "Input directory (optional)");
    public Option<string> OutputDirOption => outputDirOption ??= new(OutputDir.aliases, () => ".", "Output directory (will create if doesn't exist) (optional)");
    public Option<bool> QuietOption => quietOption ??= new(Quiet.aliases, () => false, "Do not print a message after each file is written (optional)");
    public Option<string> SearchPatternOption => searchPatternOption ??= new(SearchPattern.aliases, () => "*.json", "Filter which files to transform by name; may contain wildcards '*' (zero or more characters) and '?' (exactly one character) (optional)");
        

    private Argument<string>? inputArgument;
    private Argument<string>? outputArgument;
    private Argument<string>? csvArgument;
    private Argument<string>? suffixArgument;
    private Option<long>? startOption;
    private Option<long>? endOption;
    private Option<Format>? formatOption;
    private Option<string>? inputDirOption;
    private Option<string>? outputDirOption;
    private Option<bool>? quietOption;
    private Option<string>? searchPatternOption;
}