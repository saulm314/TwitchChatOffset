using TwitchChatOffset.CommandLine.Options;
using static TwitchChatOffset.CommandLine.Options.OptionAliases;

namespace TwitchChatOffset.Csv;

public class TransformOneToManyCsvNullables
{
    [Aliases(["output-file", "outputFile"])]
    public string? outputFile;

    [Aliases(typeof(OptionAliases), nameof(Start))]
    public long? start;

    [Aliases(typeof(OptionAliases), nameof(End))]
    public long? end;

    [Aliases(typeof(OptionAliases), nameof(PFormat))]
    public Format? format;

    [Aliases(typeof(OptionAliases), nameof(OutputDir))]
    public string? outputDir;

    [Aliases(typeof(OptionAliases), nameof(OptionPriority))]
    public long? optionPriority;
}