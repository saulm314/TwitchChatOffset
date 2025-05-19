using System.CommandLine;

namespace TwitchChatOffset;

public static class Tokens
{
    public static Argument<string> InputArgument => new("input-path", "Input path (JSON file)");
    public static Argument<string> OutputArgument => new("output-path", "Output path");
    public static Argument<string> CsvArgument => new("csv-path", "Path to the CSV file with data to transform");
    public static Option<long> StartOption => new("--start", () => 0, "Starting point in seconds before which to dismiss chat messages (optional)");
    public static Option<long> EndOption => new("--end", () => -1, "Ending point in seconds after which to dismiss chat messages (optional)");
    public static Option<Format> FormatOption => new(["--format", "--formatting", "-f"], () => default, "Format for the output file (optional)");
    public static Option<string> OutputDirOption => new(["--output", "-o"], () => ".", "Output directory (will create if doesn't exist) (optional)");
    public static Option<bool> QuietOption => new(["--quiet", "-q"], () => false, "Do not print a message after each file is written (optional)");
}