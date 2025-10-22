using TwitchChatOffset.CommandLine.Options;
using TwitchChatOffset.Ytt;
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
    
    [Aliases(typeof(OptionAliases), nameof(OptionAliases.YttMaxMessages))]
    public long? YttMaxMessages;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.YttMaxCharsPerLine))]
    public long? YttMaxCharsPerLine;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.YttScale))]
    public double? YttScale;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.YttShadow))]
    public Shadow? YttShadow;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.YttBackgroundOpacity))]
    public long? YttBackgroundOpacity;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.YttTextColor))]
    public string? YttTextColor;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.YttShadowColor))]
    public string? YttShadowColor;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.YttBackgroundColor))]
    public string? YttBackgroundColor;
    
    [Aliases(typeof(OptionAliases), nameof(OptionAliases.OutputDir))]
    public string? OutputDir;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.OptionPriority))]
    public long? OptionPriority;
}