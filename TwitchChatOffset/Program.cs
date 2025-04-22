using System;
using System.IO;
using System.CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

internal class Program
{
    private const string Version = "2.0.0";

    private static void Main(string[] args)
    {
        Console.WriteLine($"Running TwitchChatOffset {Version}");
        RootCommand rootCommand = new();
        AddOffsetCommand(rootCommand);
        AddFormatCommand(rootCommand);
        rootCommand.Invoke(args);
    }

    private static void AddOffsetCommand(RootCommand rootCommand)
    {
        Command offsetCommand = new("offset", "Offset chat based on a start time and an optional end time");
        rootCommand.Add(offsetCommand);

        Argument<string> inputArgument = new("input-file", "Input file");
        Argument<string> outputArgument = new("output-file", "Output file");
        Argument<long> startArgument = new("start", "Start time in seconds");
        Argument<long> endArgument = new("end", () => -1, "End time in seconds (optional)");
        offsetCommand.Add(inputArgument);
        offsetCommand.Add(outputArgument);
        offsetCommand.Add(startArgument);
        offsetCommand.Add(endArgument);

        offsetCommand.SetHandler(OffsetHandler.Handle, inputArgument, outputArgument, startArgument, endArgument);
    }

    private static void AddFormatCommand(RootCommand rootCommand)
    {
        Command formatCommand = new("format", "Format the JSON file into a human readable text file");
        rootCommand.Add(formatCommand);

        Argument<string> inputArgument = new("input-file", "Input file");
        Argument<string> outputArgument = new("output-file", "Output file");
        formatCommand.Add(inputArgument);
        formatCommand.Add(outputArgument);

        formatCommand.SetHandler(FormatHandler.Handle, inputArgument, outputArgument);
    }

    private static void _Main(string[] args)
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
