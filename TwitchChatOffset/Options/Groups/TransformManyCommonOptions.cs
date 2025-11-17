namespace TwitchChatOffset.Options.Groups;

public record TransformManyCommonOptions : IOptionGroup<TransformManyCommonOptions>
{
    [CliOption(nameof(CliOptions.InputFile))]
    public Plicit<string> InputFile;

    [CliOption(nameof(CliOptions.OutputFile))]
    public Plicit<string> OutputFile;

    public TransformCommonOptions TransformOptions = new();

    [CliOption(nameof(CliOptions.InputDir))]
    public Plicit<string> InputDir;

    [CliOption(nameof(CliOptions.OutputDir))]
    public Plicit<string> OutputDir;

    [CliOption(nameof(CliOptions.Suffix))]
    public Plicit<string> Suffix;
}