namespace TwitchChatOffset.Options.Groups;

public class TransformManyToManyCommonOptions : IConflictingOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformManyToManyCommonOptions));
    private static FieldData[]? _fieldDatas;

    long IConflictingOptionGroup.OptionPriority => OptionPriority;

    public TransformOptions TransformOptions = new();

    [CliOption(nameof(CliOptions.InputDir))]
    public Plicit<string> InputDir;

    [CliOption(nameof(CliOptions.OutputDir))]
    public Plicit<string> OutputDir;

    [CliOption(nameof(CliOptions.OptionPriority))]
    public Plicit<long> OptionPriority;
}