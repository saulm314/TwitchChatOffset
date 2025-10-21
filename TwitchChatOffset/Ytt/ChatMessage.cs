using System;
using YTSubConverter.Shared;

namespace TwitchChatOffset.Ytt;

public readonly record struct ChatMessage(Section Name, Section Message, TimeSpan Time);