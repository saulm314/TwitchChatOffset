using YTSubConverter.Shared;
using YTSubConverter.Shared.Formats;

namespace TwitchChatOffset.Ytt;

public class TwitchChatYttDocument : YttDocument
{
    public TwitchChatYttDocument() => LineMergeType = LineMergeType.MoveExisting;
}