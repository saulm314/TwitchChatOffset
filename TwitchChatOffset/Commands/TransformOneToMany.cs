using System.CommandLine;

namespace TwitchChatOffset.Commands;

public class TransformOneToMany
{
    public readonly Argument<string> inputArgument = Tokens.InputArgument;
    public readonly Argument<string> csvArgument = Tokens.CsvArgument;
    public readonly Option<string> outputDirOption = Tokens.OutputDirOption;
    public readonly Option<Format> formatOption = Tokens.FormatOption;
    public readonly Option<bool> quietOption = Tokens.QuietOption;

    public void Handle(Data data)
    {
        (string inputPath, string csvPath, string outputDir, Format format, bool quiet) = data;
        TransformHandler.HandleTransformOneToMany(inputPath, csvPath, outputDir, format, quiet);
    }

    public readonly record struct Data
    (
        string InputPath,
        string CsvPath,
        string OutputDir,
        Format PFormat,
        bool Quiet
    );
}