using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using TransformHandler = TwitchChatOffset.Transform;

namespace TwitchChatOffset.Commands;

public class TransformAll : CommandBinder<TransformAll.Data>
{
    public override Command PCommand { get; } = new("transform-all", "Transform all files in a directory whose name matches a search pattern");

    private readonly Tokens tokens = new();

    protected override void AddTokens()
    {
        PCommand.Add(tokens.SuffixArgument);
        PCommand.Add(tokens.InputDirOption);
        PCommand.Add(tokens.SearchPatternOption);
        PCommand.Add(tokens.OutputDirOption);
        PCommand.Add(tokens.FormatOption);
        PCommand.Add(tokens.QuietOption);
        PCommand.Add(tokens.StartOption);
        PCommand.Add(tokens.EndOption);
    }

    protected override Data GetBoundValue(BindingContext bindingContext)
    {
        T Arg<T>(Argument<T> argument) => GetArgValue(argument, bindingContext);
        T Opt<T>(Option<T> option) => GetOptValue(option, bindingContext);
        return new
        (
            Arg(tokens.SuffixArgument),
            Opt(tokens.InputDirOption),
            Opt(tokens.SearchPatternOption),
            Opt(tokens.OutputDirOption),
            Opt(tokens.FormatOption),
            Opt(tokens.QuietOption),
            Opt(tokens.StartOption),
            Opt(tokens.EndOption)
        );
    }

    public readonly record struct Data
    (
        string Suffix,
        string InputDir,
        string SearchPattern,
        string OutputDir,
        Format PFormat,
        bool Quiet,
        long Start,
        long End
    );

    protected override void Handle(Data data)
    {
        (string suffix, string inputDir, string searchPattern, string outputDir, Format format, bool quiet, long start, long end) = data;
        string[] fileNames = Directory.GetFiles(inputDir, searchPattern);
        PrintEnumerable(fileNames, "Input files found:", 0, quiet);
        _ = Directory.CreateDirectory(outputDir);
        PrintLine("Writing files...", 0, quiet);
        foreach (string fileName in fileNames)
        {
            string fileNameBody = Path.GetFileNameWithoutExtension(fileName);
            StringBuilder outputPathBuilder = new();
            outputPathBuilder.Append(outputDir);
            if (!outputDir.EndsWith('\\'))
                outputPathBuilder.Append('\\');
            outputPathBuilder.Append(fileNameBody);
            outputPathBuilder.Append(suffix);
            string outputPath = outputPathBuilder.ToString();
            PrintLine(outputPath, 2, quiet);
            string input = File.ReadAllText(fileName);
            string output;
            try
            {
                output = TransformHandler.MTransform(input, start, end, format);
            }
            catch (JsonReaderException e)
            {
                PrintError($"Could not parse JSON file {fileName}", 2);
                PrintError(e.Message, 2);
                continue;
            }
            catch (Exception e)
            {
                PrintError($"JSON file {fileName} parsed successfully but the contents were unexpected", 2);
                PrintError(e.Message, 2);
                continue;
            }
            File.WriteAllText(outputPath, output);
        }
    }
}