namespace TwitchChatOffset.Options.Groups;

public struct TransformOneToManyCliOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformOneToManyCliOptions));
    private static FieldData[]? _fieldDatas;

    public TransformOptions TransformOptions;

    [CliOption(nameof(CliOptions.OutputDir))]
    public Plicit<string> OutputDir;

    [CliOption(nameof(CliOptions.OptionPriority))]
    public Plicit<long> OptionPriority;

    [CliOption(nameof(CliOptions.Quiet))]
    public Plicit<bool> Quiet;
}