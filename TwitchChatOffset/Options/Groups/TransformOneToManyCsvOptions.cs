namespace TwitchChatOffset.Options.Groups;

public record TransformOneToManyCsvOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformOneToManyCsvOptions));
    private static FieldData[]? _fieldDatas;

    public TransformOneToManyCommonOptions CommonOptions = new();
}