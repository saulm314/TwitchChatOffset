using System;

namespace TwitchChatOffset.Csv;

public class CsvContentException : Exception
{
    public static CsvContentException DuplicateOption(string option) => new(_DuplicateOption + option);

    private const string _DuplicateOption = "CSV data contains duplicate option ";

    private CsvContentException(string? message) : base(message) { }
}