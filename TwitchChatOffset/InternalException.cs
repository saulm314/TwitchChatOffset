using System;

namespace TwitchChatOffset;

public class InternalException(string? message = null) : Exception(message);