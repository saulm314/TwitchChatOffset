using TwitchChatOffset.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public class CommentComparer : Comparer<JToken>
{
    public static readonly CommentComparer Instance = new();

    // if the chat times are more than 68 years apart, the conversion to an int will cause an overflow/underflow
    public override int Compare(JToken? x, JToken? y) => (int)(x!.D("content_offset_seconds").As<long>() - y!.D("content_offset_seconds").As<long>());
}