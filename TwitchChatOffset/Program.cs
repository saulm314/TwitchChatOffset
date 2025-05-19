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

        var inputArgument = Tokens.InputArgument;
        var outputArgument = Tokens.OutputArgument;
        var startOption = Tokens.StartOption;
        var endOption = Tokens.EndOption;
        var formatOption = Tokens.FormatOption;
        transformCommand.Add(inputArgument);
        transformCommand.Add(outputArgument);
        transformCommand.Add(startOption);
        transformCommand.Add(endOption);
        transformCommand.Add(formatOption);

        transformCommand.SetHandler(TransformHandler.HandleTransform, inputArgument, outputArgument, startOption, endOption, formatOption);
    }

    private static void AddTransformManyToManyCommand(RootCommand rootCommand)
    {
        Command transformManyToManyCommand = new("transform-many-to-many", "Transform many Json Twitch files and generate new files for each transformation");
        rootCommand.Add(transformManyToManyCommand);

        var csvArgument = Tokens.CsvArgument;
        var outputDirOption = Tokens.OutputDirOption;
        var formatOption = Tokens.FormatOption;
        var quietOption = Tokens.QuietOption;
        transformManyToManyCommand.Add(csvArgument);
        transformManyToManyCommand.Add(outputDirOption);
        transformManyToManyCommand.Add(formatOption);
        transformManyToManyCommand.Add(quietOption);

        transformManyToManyCommand.SetHandler(TransformHandler.HandleTransformManyToMany, csvArgument, outputDirOption, formatOption, quietOption);
    }

    private static void AddTransformOneToManyCommand(RootCommand rootCommand)
    {
        Command transformOneToManyCommand = new("transform-one-to-many", "Transform a JSON Twitch file into many new files");
        rootCommand.Add(transformOneToManyCommand);

        var inputArgument = Tokens.InputArgument;
        var csvArgument = Tokens.CsvArgument;
        var outputDirOption = Tokens.OutputDirOption;
        var formatOption = Tokens.FormatOption;
        var quietOption = Tokens.QuietOption;
        transformOneToManyCommand.Add(inputArgument);
        transformOneToManyCommand.Add(csvArgument);
        transformOneToManyCommand.Add(outputDirOption);
        transformOneToManyCommand.Add(formatOption);
        transformOneToManyCommand.Add(quietOption);

        transformOneToManyCommand.SetHandler(TransformHandler.HandleTransformOneToMany, inputArgument, csvArgument, outputDirOption, formatOption, quietOption);
    }
}
