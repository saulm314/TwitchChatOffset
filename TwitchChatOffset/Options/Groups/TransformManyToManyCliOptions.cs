namespace TwitchChatOffset.Options.Groups;

public class TransformManyToManyCliOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformManyToManyCliOptions));
    private static FieldData[]? _fieldDatas;

    public TransformManyToManyCommonOptions CommonOptions = new();

    [CliOption(nameof(CliOptions.Quiet))]
    public Plicit<bool> Quiet;
}