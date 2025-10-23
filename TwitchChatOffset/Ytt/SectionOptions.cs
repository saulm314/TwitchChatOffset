using System.Drawing;
using YTSubConverter.Shared;

namespace TwitchChatOffset.Ytt;

public readonly record struct SectionOptions
(
    float Scale,
    ShadowType? Shadow,
    byte BackgroundOpacity,
    Color TextColor,
    Color ShadowColor,
    Color BackgroundColor
);