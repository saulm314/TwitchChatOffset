namespace TwitchChatOffset.Options.Groups;

public record TransformOneToManyCommonOptions : IConflictingOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformOneToManyCommonOptions));
    private static FieldData[]? _fieldDatas;

    long IConflictingOptionGroup.OptionPriority => OptionPriority;

    public TransformOptions TransformOptions = new();

    [CliOption(nameof(CliOptions.OutputDir))]
    public Plicit<string> OutputDir;

    [CliOption(nameof(CliOptions.OptionPriority))]
    public Plicit<long> OptionPriority;
}