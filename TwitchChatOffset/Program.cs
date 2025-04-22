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
        Command transformCommand = AddTransformCommand(rootCommand, out Argument<string> inputArgument, out Argument<string> outputArgument);
        _ = AddOffsetCommand(transformCommand, inputArgument, outputArgument, out _, out _);
        _ = AddFormatCommand(transformCommand, inputArgument, outputArgument);
        rootCommand.Invoke(args);
    }

    private static Command AddTransformCommand(RootCommand rootCommand, out Argument<string> inputArgument, out Argument<string> outputArgument)
    {
        Command transformCommand = new("transform", "Transform a JSON Twitch chat file and put the new contents in a new file");
        rootCommand.Add(transformCommand);

        inputArgument = new("input-path", "Input path");
        outputArgument = new("output-path", "Output path");
        transformCommand.Add(inputArgument);
        transformCommand.Add(outputArgument);

        return transformCommand;
    }

    private static Command AddOffsetCommand(Command transformCommand, Argument<string> inputArgument, Argument<string> outputArgument,
        out Argument<long> startArgument, out Argument<long> endArgument)
    {
        Command offsetCommand = new("offset", "Offset chat given optional start and end times");
        transformCommand.Add(offsetCommand);

        startArgument = new("start", () => 0, "Start time in seconds (optional)");
        endArgument = new("end", () => -1, "End time in seconds (optional)");
        offsetCommand.Add(startArgument);
        offsetCommand.Add(endArgument);

        offsetCommand.SetHandler(CommandHandler.HandleOffset, inputArgument, outputArgument, startArgument, endArgument);
        return offsetCommand;
    }

    private static Command AddFormatCommand(Command transformCommand, Argument<string> inputArgument, Argument<string> outputArgument)
    {
        Command formatCommand = new("format", "Format the JSON text file and put the new contents into a new file");
        transformCommand.Add(formatCommand);

        formatCommand.SetHandler(CommandHandler.HandleFormat, inputArgument, outputArgument);
        return formatCommand;
    }
}
