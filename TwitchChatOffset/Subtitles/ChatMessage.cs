using System;
using YTSubConverter.Shared;

namespace TwitchChatOffset.Subtitles;

public readonly record struct ChatMessage(Section Name, Section Message, TimeSpan Time);