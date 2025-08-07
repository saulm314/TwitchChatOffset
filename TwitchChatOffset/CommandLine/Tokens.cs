using TwitchChatOffset.CommandLine.Options;
using System.CommandLine;
using static TwitchChatOffset.CommandLine.Options.OptionAliases;

namespace TwitchChatOffset.CommandLine;

public class Tokens
{
    public Argument<string> InputArgument => _inputArgument ??=
        new("input-path", "Input path (JSON file)");

    public Argument<string> OutputArgument => _outputArgument ??=
        new("output-path", "Output path");

    public Argument<string> CsvArgument => _csvArgument ??=
        new("csv-path", "Path to the CSV file with data to transform");

    public Argument<string> SuffixArgument => _suffixArgument ??=
        new("suffix", "Suffix to be apended to all output file names, including the extension");
    
    public OptionContainer<long> StartOption => _startOption ??=
        new(Start, 0, "Starting point in seconds before which to dismiss chat messages (optional)");

    public OptionContainer<long> EndOption => _endOption ??=
        new(End, -1, "Ending point in seconds after which to dismiss chat messages, or any negative number for no ending point (optional)");

    public OptionContainer<Format> FormatOption => _formatOption ??=
        new(OptionAliases.Format, default, "Format for the output file (optional)");

    public OptionContainer<string> OutputDirOption => _outputDirOption ??=
        new(OutputDir, ".", "Output directory (will create if doesn't exist) (optional)");
    
    public OptionContainer<string> InputDirOption => _inputDirOption ??=
        new(InputDir, ".", "Input directory (optional)");

    public OptionContainer<bool> QuietOption => _quietOption ??=
        new(Quiet, false, "Do not print a message after each file is written (optional)");

    public OptionContainer<string> SearchPatternOption => _searchPatternOption ??=
        new(SearchPattern, "*.json", "Filter which files to transform by name; may contain wildcards '*' (zero or more characters) and '?' (exactly one character) (optional)");

    public OptionContainer<long> OptionPriorityOption => _optionPriorityOption ??=
        new(OptionAliases.OptionPriority, 0, "Select priority to determine which options should be used when there is a clash between the CLI options and the CSV options (can be any integer; higher priority wins; if priorities are equal, then CSV is prioritised (optional)");

    private Argument<string>? _inputArgument;
    private Argument<string>? _outputArgument;
    private Argument<string>? _csvArgument;
    private Argument<string>? _suffixArgument;

    private OptionContainer<long>? _startOption;
    private OptionContainer<long>? _endOption;
    private OptionContainer<Format>? _formatOption;
    private OptionContainer<string>? _outputDirOption;
    private OptionContainer<string>? _inputDirOption;
    private OptionContainer<bool>? _quietOption;
    private OptionContainer<string>? _searchPatternOption;
    private OptionContainer<long>? _optionPriorityOption;
}