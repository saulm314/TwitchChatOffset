using System.CommandLine;

namespace TwitchChatOffset;

internal class Program
{
    private static void Main(string[] args)
    {
        RootCommand rootCommand = new("Tools for handling Twitch chat JSON files");
        AddTransformCommand(rootCommand);
        AddTransformManyToManyCommand(rootCommand);
        AddTransformOneToManyCommand(rootCommand);
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
        Option<Formatting> formattingOption = new("--formatting", () => default, "Formatting for the output file (optional)");
        transformCommand.Add(inputArgument);
        transformCommand.Add(outputArgument);
        transformCommand.Add(startOption);
        transformCommand.Add(endOption);
        transformCommand.Add(formattingOption);

        transformCommand.SetHandler(TransformHandler.HandleTransform, inputArgument, outputArgument, startOption, endOption, formattingOption);
    }

    private static void AddTransformManyToManyCommand(RootCommand rootCommand)
    {
        Command transformManyToManyCommand = new("transform-many-to-many", "Transform many Json Twitch files and generate new files for each transformation");
        rootCommand.Add(transformManyToManyCommand);

        Argument<string> csvArgument = new("csv-path", "Path to the CSV file with data to transform");
        Option<string> outputOption = new(["--output", "-o"], () => ".", "Output directory (will create if doesn't exist) (optional)");
        Option<Formatting> formattingOption = new("--formatting", () => default, "Formatting for the output file (optional)");
        transformManyToManyCommand.Add(csvArgument);
        transformManyToManyCommand.Add(outputOption);
        transformManyToManyCommand.Add(formattingOption);

        transformManyToManyCommand.SetHandler(TransformHandler.HandleTransformManyToMany, csvArgument, outputOption, formattingOption);
    }

    private static void AddTransformOneToManyCommand(RootCommand rootCommand)
    {
        Command transformOneToManyCommand = new("transform-one-to-many", "Transform a JSON Twitch file into many new files");
        rootCommand.Add(transformOneToManyCommand);

        Argument<string> inputArgument = new("input-path", "Input path (JSON file)");
        Argument<string> csvArgument = new("csv-path", "Path to the CSV file with data to transform");
        Option<string> outputOption = new(["--output", "-o"], () => ".", "Output directory (will create if doesn't exist) (optional)");
        Option<Formatting> formattingOption = new("--formatting", () => default, "Formatting for the output file (optional)");
        transformOneToManyCommand.Add(inputArgument);
        transformOneToManyCommand.Add(csvArgument);
        transformOneToManyCommand.Add(outputOption);
        transformOneToManyCommand.Add(formattingOption);

        transformOneToManyCommand.SetHandler(TransformHandler.HandleTransformOneToMany, inputArgument, csvArgument, outputOption, formattingOption);
    }
}
