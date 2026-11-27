using TwitchChatOffset.ConsoleUtils;

namespace TwitchChatOffset.Options.Groups;

public record TransformManyCliOptions : OptionGroup<TransformManyCliOptions>
{
    public TransformManyCommonOptions CommonOptions = new();

    [CliOption(nameof(CliOptions.Quiet))]
    public Plicit<bool> Quiet;

    [CliOption(nameof(CliOptions.Suppress))]
    public Plicit<bool> Suppress;

    [CliOption(nameof(CliOptions.MultiResponse))]
    public Plicit<CliMultiResponse> Response;
}