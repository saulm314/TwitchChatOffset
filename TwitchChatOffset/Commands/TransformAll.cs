using System.CommandLine;

namespace TwitchChatOffset.Commands;

public class TransformAll
{
    public readonly Argument<string> suffixArgument = Tokens.SuffixArgument;
    public readonly Option<string> inputDirOption = Tokens.InputDirOption;
    public readonly Option<string> searchPatternOption = Tokens.SearchPatternOption;
    public readonly Option<string> outputDirOption = Tokens.OutputDirOption;
    public readonly Option<Format> formatOption = Tokens.FormatOption;
    public readonly Option<bool> quietOption = Tokens.QuietOption;
    public readonly Option<long> startOption = Tokens.StartOption;
    public readonly Option<long> endOption = Tokens.EndOption;

    public void Handle(Data data)
    {
        (string suffix, string inputDir, string searchPattern, string outputDir, Format format, bool quiet, long start, long end) = data;
        TransformHandler.HandleTransformAll(suffix, inputDir, searchPattern, outputDir, format, quiet, start, end);
    }

    public readonly record struct Data
    (
        string Suffix,
        string InputDir,
        string SearchPattern,
        string OutputDir,
        Format PFormat,
        bool Quiet,
        long Start,
        long End
    );
}