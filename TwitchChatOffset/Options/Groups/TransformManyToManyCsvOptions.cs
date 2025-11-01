namespace TwitchChatOffset.Options.Groups;

public struct TransformManyToManyCsvOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformManyToManyCsvOptions));
    private static FieldData[]? _fieldDatas;

    [Aliases(["input-file", "inputFile"])]
    public Plicit<string> InputFile;

    [Aliases(["output-file", "outputFile"])]
    public Plicit<string> OutputFile;

    public TransformOptions TransformOptions;

    [CliOption(nameof(CliOptions.InputDir))]
    public Plicit<string> InputDir;

    [CliOption(nameof(CliOptions.OutputDir))]
    public Plicit<string> OutputDir;

    [CliOption(nameof(CliOptions.OptionPriority))]
    public Plicit<long> OptionPriority;
}