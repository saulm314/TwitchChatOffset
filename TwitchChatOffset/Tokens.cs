using System.CommandLine;

namespace TwitchChatOffset;

public class Tokens
{
    public Argument<string> InputArgument => inputArgument ??= new("input-path", "Input path (JSON file)");
    public Argument<string> OutputArgument => outputArgument ??= new("output-path", "Output path");
    public Argument<string> CsvArgument => csvArgument ??= new("csv-path", "Path to the CSV file with data to transform");
    public Argument<string> SuffixArgument => suffixArgument ??= new("suffix", "Suffix to be apended to all output file names, including the extension");
    public Option<long> StartOption => startOption ??= new("--start", () => 0, "Starting point in seconds before which to dismiss chat messages (optional)");
    public Option<long> EndOption => endOption ??= new("--end", () => -1, "Ending point in seconds after which to dismiss chat messages (optional)");
    public Option<Format> FormatOption => formatOption ??= new(["--format", "--formatting", "-f"], () => default, "Format for the output file (optional)");
    public Option<string> InputDirOption => inputDirOption ??= new(["--input", "--input-directory", "-i"], () => ".", "Input directory (optional)");
    public Option<string> OutputDirOption => outputDirOption ??= new(["--output", "--output-directory", "-o"], () => ".", "Output directory (will create if doesn't exist) (optional)");
    public Option<bool> QuietOption => quietOption ??= new(["--quiet", "-q"], () => false, "Do not print a message after each file is written (optional)");
    public Option<string> SearchPatternOption => searchPatternOption ??= new("--search-pattern", () => "*.json", "Filter which files to transform by name; may contain wildcards '*' and '?' (optional)");

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