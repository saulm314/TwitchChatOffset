using System;

namespace TwitchChatOffset;

public class ParserException(string? message = null) : Exception(message);