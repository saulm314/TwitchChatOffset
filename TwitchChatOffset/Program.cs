using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset
{
    internal class Program
    {
        const string Version = "1.1.0";

        static void Main(string[] args)
        {
            Console.WriteLine($"Running TwitchChatOffset {Version}");
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide arguments: <input json path> <output json path> <start (seconds)> [end (seconds)]");
                return;
            }

            if (args.Length != 3 && args.Length != 4)
            {
                Console.WriteLine("Unexpected number of arguments");
                return;
            }

            string inputPath = args[0];
            string outputPath = args[1];
            bool startIsLong = long.TryParse(args[2], out long start);
            long end = -1;

            if (!startIsLong)
            {
                Console.WriteLine("Integer expected for start");
                return;
            }

            if (args.Length == 4)
            {
                bool endIsLong = long.TryParse(args[3], out end);
                if (!endIsLong)
                {
                    Console.WriteLine("Integer expected for end");
                    return;
                }
            }

            JToken parent = (JToken)JsonConvert.DeserializeObject(File.ReadAllText(inputPath))!;
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

            Console.WriteLine("Done.");
        }
    }
}
