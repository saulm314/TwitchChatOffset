namespace TwitchChatOffset.Options.Groups;

public record TransformManyCsvOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformManyCsvOptions));
    private static FieldData[]? _fieldDatas;

    public TransformManyCommonOptions CommonOptions = new();
}