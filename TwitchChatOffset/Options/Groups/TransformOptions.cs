using TwitchChatOffset.ConsoleUtils;

namespace TwitchChatOffset.Options.Groups;

public record TransformOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformOptions));
    private static FieldData[]? _fieldDatas;

    public TransformCommonOptions Options = new();

    [CliOption(nameof(CliOptions.Response))]
    public Plicit<CliResponse> Response;
}