using System.CommandLine;

namespace TwitchChatOffset;

internal class Program
{
    private static void Main(string[] args)
    {
        RootCommand rootCommand = new("Tools for handling Twitch chat JSON files");
        AddTransformCommand(rootCommand);
        AddGetVideosCommand(rootCommand);
        rootCommand.Invoke(args);
    }

    private static void AddTransformCommand(RootCommand rootCommand)
    {
        Command transformCommand = new("transform", "Transform a JSON Twitch chat file and put the new contents in a new file");
        rootCommand.Add(transformCommand);

        Argument<string> inputArgument = new("input-path", "Input path");
        Argument<string> outputArgument = new("output-path", "Output path");
        Option<long> startOption = new("--start", () => 0, "Starting point in seconds before which to dismiss chat messages (optional)");
        Option<long> endOption = new("--end", () => -1, "Ending point in seconds after which to dismiss chat messages (optional)");
        Option<TransformFormatting> formattingOption = new("--formatting", () => default, "Formatting for the output file (optional)");
        transformCommand.Add(inputArgument);
        transformCommand.Add(outputArgument);
        transformCommand.Add(startOption);
        transformCommand.Add(endOption);
        transformCommand.Add(formattingOption);

        transformCommand.SetHandler(TransformHandler.HandleTransform, inputArgument, outputArgument, startOption, endOption, formattingOption);
    }

    private static void AddGetVideosCommand(RootCommand rootCommand)
    {
        Command getVideosCommand = new("get-videos", "[placeholder]");
        rootCommand.Add(getVideosCommand);

        Argument<string> usernameArgument = new("username", "[placeholder]");
        Argument<string> outputArgument = new("output-path", "[placeholder]");
        Option<GetVideosFormatting> formattingOption = new("--formatting", () => default, "[placeholder]");
        getVideosCommand.Add(usernameArgument);
        getVideosCommand.Add(outputArgument);
        getVideosCommand.Add(formattingOption);

        getVideosCommand.SetHandler(GetVideosHandler.HandleGetVideos, usernameArgument, outputArgument, formattingOption);
    }
}
