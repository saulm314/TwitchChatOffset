using System;
using System.CommandLine;
using System.Reflection;

namespace TwitchChatOffset;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.Write("Running TwitchChatOffset ");
        string version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "[unknown version]";
        Console.WriteLine(version);

        RootCommand rootCommand = new("Tools for handling Twitch chat JSON files");
        AddOffsetCommand(rootCommand);
        AddFormatCommand(rootCommand);
        rootCommand.Invoke(args);
    }

    private static void AddOffsetCommand(RootCommand rootCommand)
    {
        Command offsetCommand = new("offset", "Offset chat based on a start time and an optional end time");
        rootCommand.Add(offsetCommand);

        Argument<string> inputArgument = new("input-path", "Input path");
        Argument<string> outputArgument = new("output-path", "Output path");
        Argument<long> startArgument = new("start", "Start time in seconds");
        Argument<long> endArgument = new("end", () => -1, "End time in seconds (optional)");
        offsetCommand.Add(inputArgument);
        offsetCommand.Add(outputArgument);
        offsetCommand.Add(startArgument);
        offsetCommand.Add(endArgument);

        offsetCommand.SetHandler(CommandHandler.HandleOffset, inputArgument, outputArgument, startArgument, endArgument);
    }

    private static void AddFormatCommand(RootCommand rootCommand)
    {
        Command formatCommand = new("format", "Format the JSON file into a human readable text file");
        rootCommand.Add(formatCommand);

        Argument<string> inputArgument = new("input-path", "Input path");
        Argument<string> outputArgument = new("output-path", "Output path");
        formatCommand.Add(inputArgument);
        formatCommand.Add(outputArgument);

        formatCommand.SetHandler(CommandHandler.HandleFormat, inputArgument, outputArgument);
    }
}
