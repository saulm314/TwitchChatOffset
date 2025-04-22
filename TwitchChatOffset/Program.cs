using System;
using System.CommandLine;
using System.Reflection;

namespace TwitchChatOffset;

internal class Program
{
    private static void Main(string[] args)
    {
        RootCommand rootCommand = new("Tools for handling Twitch chat JSON files");
        Command transformCommand = AddTransformCommand(rootCommand,
            out Argument<string> inputArgument, out Argument<string> outputArgument, out Option<Formatting> formattingOption);
        _ = AddOffsetCommand(transformCommand, inputArgument, outputArgument, formattingOption, out _, out _);
        _ = AddNoTransformationCommand(transformCommand, inputArgument, outputArgument, formattingOption);
        rootCommand.Invoke(args);
    }

    private static Command AddTransformCommand(RootCommand rootCommand,
        out Argument<string> inputArgument, out Argument<string> outputArgument, out Option<Formatting> formattingOption)
    {
        Command transformCommand = new("transform", "Transform a JSON Twitch chat file and put the new contents in a new file");
        rootCommand.Add(transformCommand);

        inputArgument = new("input-path", "Input path");
        outputArgument = new("output-path", "Output path");
        transformCommand.Add(inputArgument);
        transformCommand.Add(outputArgument);

        formattingOption = new("--formatting", () => default, "Formatting for the output file");
        transformCommand.AddGlobalOption(formattingOption);

        return transformCommand;
    }

    private static Command AddOffsetCommand(Command transformCommand,
        Argument<string> inputArgument, Argument<string> outputArgument, Option<Formatting> formattingOption,
        out Argument<long> startArgument, out Argument<long> endArgument)
    {
        Command offsetCommand = new("offset", "Offset chat given optional start and end times");
        transformCommand.Add(offsetCommand);

        startArgument = new("start", () => 0, "Start time in seconds (optional)");
        endArgument = new("end", () => -1, "End time in seconds (optional)");
        offsetCommand.Add(startArgument);
        offsetCommand.Add(endArgument);

        offsetCommand.SetHandler(CommandHandler.HandleOffset, inputArgument, outputArgument, startArgument, endArgument, formattingOption);
        return offsetCommand;
    }

    private static Command AddNoTransformationCommand(Command transformCommand,
        Argument<string> inputArgument, Argument<string> outputArgument, Option<Formatting> formattingOption)
    {
        Command noTransformationCommand = new("no-transformation", "Do not apply any transformations to the information in the file (e.g. useful if you just want to change the formatting)");
        transformCommand.Add(noTransformationCommand);

        noTransformationCommand.SetHandler(CommandHandler.HandleNoTransformation, inputArgument, outputArgument, formattingOption);
        return noTransformationCommand;
    }
}
