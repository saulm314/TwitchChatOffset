using static TwitchChatOffset.OptionAliases;

namespace TwitchChatOffset;

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
    
    [Aliases(typeof(OptionAliases), nameof(PFormat))]
    public Format? format;
    
    [Aliases(typeof(OptionAliases), nameof(OutputDir))]
    public string? outputDir;
}