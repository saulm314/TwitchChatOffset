namespace TwitchChatOffset.Options.Groups;

public record TransformOneToManyCsvOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformOneToManyCsvOptions));
    private static FieldData[]? _fieldDatas;

    [Aliases(["output-file", "outputFile"])]
    public Plicit<string> OutputFile;

    public TransformOneToManyCommonOptions CommonOptions = new();
}