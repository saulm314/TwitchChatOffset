namespace TwitchChatOffset.Options.Groups;

public record TransformOneToManyCommonOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformOneToManyCommonOptions));
    private static FieldData[]? _fieldDatas;

    public TransformOptions TransformOptions = new();

    [CliOption(nameof(CliOptions.OutputDir))]
    public Plicit<string> OutputDir;

    [CliOption(nameof(CliOptions.Suffix))]
    public Plicit<string> Suffix;
}