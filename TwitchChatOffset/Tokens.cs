using System.Collections.Generic;
using System.CommandLine;

namespace TwitchChatOffset;

public class Tokens
{
    public Argument<string> InputArgument => inputArgument ??= new("input-path", "Input path (JSON file)");
    public Argument<string> OutputArgument => outputArgument ??= new("output-path", "Output path");
    public Argument<string> CsvArgument => csvArgument ??= new("csv-path", "Path to the CSV file with data to transform");
    public Argument<string> SuffixArgument => suffixArgument ??= new("suffix", "Suffix to be apended to all output file names, including the extension");
    public Option<long> StartOption => startOption ??= new(StartOptionAliases, () => 0, "Starting point in seconds before which to dismiss chat messages (optional)");
    public Option<long> EndOption => endOption ??= new(EndOptionAliases, () => -1, "Ending point in seconds after which to dismiss chat messages (optional)");
    public Option<Format> FormatOption => formatOption ??= new(FormatOptionAliases, () => default, "Format for the output file (optional)");
    public Option<string> InputDirOption => inputDirOption ??= new(InputDirOptionAliases, () => ".", "Input directory (optional)");
    public Option<string> OutputDirOption => outputDirOption ??= new(OutputDirOptionAliases, () => ".", "Output directory (will create if doesn't exist) (optional)");
    public Option<bool> QuietOption => quietOption ??= new(QuietOptionAliases, () => false, "Do not print a message after each file is written (optional)");
    public Option<string> SearchPatternOption => searchPatternOption ??= new(SearchPatternOptionAliases, () => "*.json", "Filter which files to transform by name; may contain wildcards '*' and '?' (optional)");

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

    public static string[] StartOptionAliases { get; } = ["--start"];
    public static string[] EndOptionAliases { get; } = ["--end"];
    public static string[] FormatOptionAliases { get; } = ["--format", "--formatting", "-f"];
    public static string[] InputDirOptionAliases { get; } = ["--input-directory", "--input", "-i"];
    public static string[] OutputDirOptionAliases { get; } = ["--output-directory", "--output", "-o"];
    public static string[] QuietOptionAliases { get; } = ["--quiet", "-q"];
    public static string[] SearchPatternOptionAliases { get; } = ["--search-pattern"];
}