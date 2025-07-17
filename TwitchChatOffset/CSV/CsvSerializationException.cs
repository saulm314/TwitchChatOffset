using System;

namespace TwitchChatOffset.CSV;

public class CsvSerializationException(string? message) : Exception(message);