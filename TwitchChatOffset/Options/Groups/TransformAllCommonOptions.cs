namespace TwitchChatOffset.Options.Groups;

public record TransformAllCommonOptions : OptionGroup<TransformAllCommonOptions>
{
    public TransformCommonOptions TransformOptions = new();

    [CliOption(nameof(CliOptions.InputDir))]
    public Plicit<string> InputDir;

    [CliOption(nameof(CliOptions.SearchPattern))]
    public Plicit<string> SearchPattern;

    [CliOption(nameof(CliOptions.OutputDir))]
    public Plicit<string> OutputDir;

    [CliOption(nameof(CliOptions.Suffix))]
    public Plicit<string> Suffix;
}