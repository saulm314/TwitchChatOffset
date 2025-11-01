namespace TwitchChatOffset.Options.Groups;

public class TransformManyToManyCsvOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformManyToManyCsvOptions));
    private static FieldData[]? _fieldDatas;

    [Aliases(["input-file", "inputFile"])]
    public Plicit<string> InputFile;

    [Aliases(["output-file", "outputFile"])]
    public Plicit<string> OutputFile;

    public TransformManyToManyCommonOptions CommonOptions = new();
}