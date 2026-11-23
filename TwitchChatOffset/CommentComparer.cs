using TwitchChatOffset.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public class CommentComparer : Comparer<JToken>
{
    public static readonly CommentComparer Instance = new();

    public override int Compare(JToken? x, JToken? y) => x!.D<long>("content_offset_seconds").CompareTo(y!.D<long>("content_offset_seconds"));
}