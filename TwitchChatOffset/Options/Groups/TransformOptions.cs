using TwitchChatOffset.ConsoleUtils;

namespace TwitchChatOffset.Options.Groups;

public record TransformOptions : IOptionGroup<TransformOptions>
{
    public TransformCommonOptions Options = new();

    [CliOption(nameof(CliOptions.Response))]
    public Plicit<CliResponse> Response;
}