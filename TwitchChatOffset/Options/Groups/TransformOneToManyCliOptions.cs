namespace TwitchChatOffset.Options.Groups;

public record TransformOneToManyCliOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformOneToManyCliOptions));
    private static FieldData[]? _fieldDatas;

    public TransformOneToManyCommonOptions CommonOptions = new();

    [CliOption(nameof(CliOptions.Quiet))]
    public Plicit<bool> Quiet;
}