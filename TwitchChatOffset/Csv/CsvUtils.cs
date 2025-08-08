using CSVFile;

namespace TwitchChatOffset.Csv;

public static class CsvUtils
{
    public static readonly CSVSettings CsvSettings = new()
    {
        FieldDelimiter = ','
    };
}