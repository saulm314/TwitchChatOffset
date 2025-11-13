namespace TwitchChatOffset.Options.Groups;

public record TransformManyToManyCsvOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformManyToManyCsvOptions));
    private static FieldData[]? _fieldDatas;

    public TransformManyToManyCommonOptions CommonOptions = new();
}