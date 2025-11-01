namespace TwitchChatOffset.Options.Groups;

public struct TransformOneToManyCsvOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformOneToManyCsvOptions));
    private static FieldData[]? _fieldDatas;

    [Aliases(["output-file", "outputFile"])]
    public Plicit<string> OutputFile;

    public TransformOptions TransformOptions;

    [CliOption(nameof(CliOptions.OutputDir))]
    public Plicit<string> OutputDir;

    [CliOption(nameof(CliOptions.OptionPriority))]
    public Plicit<long> OptionPriority;
}