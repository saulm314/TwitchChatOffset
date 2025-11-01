namespace TwitchChatOffset.Options.Groups;

public struct TransformManyToManyCliOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformManyToManyCliOptions));
    private static FieldData[]? _fieldDatas;

    public TransformOptions TransformOptions;

    [CliOption(nameof(CliOptions.InputDir))]
    public Plicit<string> InputDir;

    [CliOption(nameof(CliOptions.OutputDir))]
    public Plicit<string> OutputDir;

    [CliOption(nameof(CliOptions.OptionPriority))]
    public Plicit<long> OptionPriority;

    [CliOption(nameof(CliOptions.Quiet))]
    public Plicit<bool> Quiet;
}