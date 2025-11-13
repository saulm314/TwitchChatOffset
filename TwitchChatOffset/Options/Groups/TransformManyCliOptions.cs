using TwitchChatOffset.ConsoleUtils;

namespace TwitchChatOffset.Options.Groups;

public record TransformManyCliOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformManyCliOptions));
    private static FieldData[]? _fieldDatas;

    public TransformManyCommonOptions CommonOptions = new();

    [CliOption(nameof(CliOptions.Quiet))]
    public Plicit<bool> Quiet;

    [CliOption(nameof(CliOptions.MultiResponse))]
    public Plicit<CliMultiResponse> Response;
}