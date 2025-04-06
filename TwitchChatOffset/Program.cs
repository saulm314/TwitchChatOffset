using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running Twitch Chat Offset...");
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide arguments: [input json path] [output json path] [offset (seconds)]");
                return;
            }

            if (args.Length != 3)
            {
                Console.WriteLine("Unexpected number of arguments");
                return;
            }

            string inputPath = args[0];
            string outputPath = args[1];
            bool offsetIsLong = long.TryParse(args[2], out long offset);

            if (!offsetIsLong)
            {
                Console.WriteLine("Integer expected for offset");
                return;
            }

            JToken parent = (JToken)JsonConvert.DeserializeObject(File.ReadAllText(inputPath))!;
            JArray comments = (JArray)parent["comments"]!;
            int i = 0;
            while (i < comments.Count)
            {
                JToken comment = comments[i];
                JValue commentOffset = (JValue)comment["content_offset_seconds"]!;
                long commentOffsetValue = (long)commentOffset.Value!;
                if (commentOffsetValue! < offset)
                {
                    comments.RemoveAt(i);
                    continue;
                }
                commentOffset.Value = commentOffsetValue - offset;
                i++;
            }

            string output = JsonConvert.SerializeObject(parent);
            File.WriteAllText(outputPath, output);

            Console.WriteLine("Done.");
        }
    }
}
