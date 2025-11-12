namespace TwitchChatOffset.Options.Groups;

public record TransformManyToManyCommonOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformManyToManyCommonOptions));
    private static FieldData[]? _fieldDatas;

    public TransformOptions TransformOptions = new();

    [CliOption(nameof(CliOptions.InputDir))]
    public Plicit<string> InputDir;

    [CliOption(nameof(CliOptions.OutputDir))]
    public Plicit<string> OutputDir;
}