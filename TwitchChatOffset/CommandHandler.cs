using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public static class CommandHandler
{
    public static void HandleOffset(string inputPath, string outputPath, long start = 0, long end = -1)
    {
        string input = File.ReadAllText(inputPath);
        JToken parent = (JToken)JsonConvert.DeserializeObject(input)!;
        JArray comments = (JArray)parent["comments"]!;
        int i = 0;
        while (i < comments.Count)
        {
            JToken comment = comments[i];
            JValue commentOffset = (JValue)comment["content_offset_seconds"]!;
            long commentOffsetValue = (long)commentOffset.Value!;
            if (commentOffsetValue < start)
            {
                comments.RemoveAt(i);
                continue;
            }
            if (end != -1 && commentOffsetValue > end)
            {
                comments.RemoveAt(i);
                continue;
            }
            commentOffset.Value = commentOffsetValue - start;
            i++;
        }
        string output = JsonConvert.SerializeObject(parent);
        File.WriteAllText(outputPath, output);
    }

    public static void HandleFormat(string inputPath, string outputPath)
    {

    }
}