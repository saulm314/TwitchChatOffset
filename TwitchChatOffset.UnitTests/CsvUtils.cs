using CSVFile;

namespace TwitchChatOffset.UnitTests;

public static class CsvUtils
{
    public static readonly CSVSettings csvSettings = new()
    {
        FieldDelimiter = ','
    };
}