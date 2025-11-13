using TwitchChatOffset.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public class CommentComparer : Comparer<JToken>
{
    public static readonly CommentComparer Instance = new();

    public override int Compare(JToken? x, JToken? y)
    {
        long diff = x!.D("content_offset_seconds").As<long>() - y!.D("content_offset_seconds").As<long>();
        return diff switch
        {
            < 0 => -1,
            > 0 => 1,
            0 => 0
        };
    }
}