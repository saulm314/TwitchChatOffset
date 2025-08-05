using System;

namespace TwitchChatOffset.Csv;

public abstract class CsvContentException(string? message) : Exception(message)
{
    public class DuplicateOption(string option) : CsvContentException($"CSV data contains duplicate option {option}")
    {
        public string Option => option;
    }
}