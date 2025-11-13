namespace TwitchChatOffset.Options.Groups;

public record TransformCommonOptions : IOptionGroup
{
    public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(TransformCommonOptions));
    private static FieldData[]? _fieldDatas;

    [CliOption(nameof(CliOptions.Start))]
    public Plicit<long> Start;

    [CliOption(nameof(CliOptions.End))]
    public Plicit<long> End;

    [CliOption(nameof(CliOptions.Delay))]
    public Plicit<long> Delay;

    [CliOption(nameof(CliOptions.Format))]
    public Plicit<Format> Format;

    public SubtitleOptions SubtitleOptions = new();

    public void Deconstruct(out long start, out long end, out long delay)
    {
        start = Start;
        end = End;
        delay = Delay;
    }
}