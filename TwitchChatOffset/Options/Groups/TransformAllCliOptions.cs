using TwitchChatOffset.ConsoleUtils;

namespace TwitchChatOffset.Options.Groups;

public record TransformAllCliOptions : OptionGroup<TransformAllCliOptions>
{
    public TransformAllCommonOptions CommonOptions = new();

    [CliOption(nameof(CliOptions.Quiet))]
    public Plicit<bool> Quiet;

    [CliOption(nameof(CliOptions.Suppress))]
    public Plicit<bool> Suppress;

    [CliOption(nameof(CliOptions.MultiResponse))]
    public Plicit<CliMultiResponse> Response;

    [CliOption(nameof(CliOptions.CsvPath))]
    public Plicit<string> CsvPath;

    [CliOption(nameof(CliOptions.CliDelimiter))]
    public Plicit<string> CliDelimiter;

    [CliOption(nameof(CliOptions.CsvFieldDelimiter))]
    public Plicit<char> CsvFieldDelimiter;
}