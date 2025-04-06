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
            bool offsetIsInt = int.TryParse(args[2], out int offset);

            if (!offsetIsInt)
            {
                Console.WriteLine("Integer expected for offset");
                return;
            }
        }
    }
}
