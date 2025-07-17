using CSVFile;

namespace TwitchChatOffset.CSV;

public static class CsvUtils
{
    public static readonly CSVSettings csvSettings = new()
    {
        FieldDelimiter = ','
    };
}