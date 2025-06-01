namespace TwitchChatOffset;

public abstract class BulkTransformCsv
{
    public string? outputFile;
    public long? start;
    public long? end;
    public Format? format;
    public string? outputDir;
}