using TwitchChatOffset.CommandLine.Options;
using YTSubConverter.Shared;

namespace TwitchChatOffset.Csv;

public class TransformManyToManyCsvNullables
{
    [Aliases(["input-file", "inputFile"])]
    public string? InputFile;

    [Aliases(["output-file", "outputFile"])]
    public string? OutputFile;
    
    [Aliases(typeof(OptionAliases), nameof(OptionAliases.Start))]
    public long? Start;
    
    [Aliases(typeof(OptionAliases), nameof(OptionAliases.End))]
    public long? End;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.Delay))]
    public long? Delay;
    
    [Aliases(typeof(OptionAliases), nameof(OptionAliases.Format))]
    public Format? Format;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.YttPosition))]
    public AnchorPoint? YttPosition;
    
    [Aliases(typeof(OptionAliases), nameof(OptionAliases.OutputDir))]
    public string? OutputDir;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.OptionPriority))]
    public long? OptionPriority;
}