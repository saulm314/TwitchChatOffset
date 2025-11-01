using System.CommandLine;

namespace TwitchChatOffset.CommandLine;

public static class Arguments
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
}