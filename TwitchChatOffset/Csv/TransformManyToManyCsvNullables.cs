using TwitchChatOffset.CommandLine.Options;
using static TwitchChatOffset.CommandLine.Options.OptionAliases;

namespace TwitchChatOffset.Csv;

public class TransformManyToManyCsvNullables
{
    [Aliases(["input-file", "inputFile"])]
    public string? inputFile;

    [Aliases(["output-file", "outputFile"])]
    public string? outputFile;
    
    [Aliases(typeof(OptionAliases), nameof(Start))]
    public long? start;
    
    [Aliases(typeof(OptionAliases), nameof(End))]
    public long? end;
    
    [Aliases(typeof(OptionAliases), nameof(OptionAliases.Format))]
    public Format? format;
    
    [Aliases(typeof(OptionAliases), nameof(OutputDir))]
    public string? outputDir;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.OptionPriority))]
    public long? optionPriority;
}