using System;

namespace TwitchChatOffset.CommandLine;

public class CliException(string? message = null) : Exception(message);