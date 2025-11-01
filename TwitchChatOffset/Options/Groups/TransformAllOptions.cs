namespace TwitchChatOffset.Options.Groups;

public class TransformAllOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformAllOptions));
    private static FieldData[]? _fieldDatas;

    public TransformOptions TransformOptions = new();

    [CliOption(nameof(CliOptions.InputDir))]
    public Plicit<string> InputDir;

    [CliOption(nameof(CliOptions.SearchPattern))]
    public Plicit<string> SearchPattern;

    [CliOption(nameof(CliOptions.OutputDir))]
    public Plicit<string> OutputDir;

    [CliOption(nameof(CliOptions.Quiet))]
    public Plicit<bool> Quiet;
}