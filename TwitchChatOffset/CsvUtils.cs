using CSVFile;

namespace TwitchChatOffset;

public static class CsvUtils
{
    public static readonly CSVSettings csvSettings = new()
    {
        FieldDelimiter = ','
    };
}