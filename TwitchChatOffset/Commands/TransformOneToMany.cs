using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using CSVFile;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TransformHandler = TwitchChatOffset.Transform;

namespace TwitchChatOffset.Commands;

public class TransformOneToMany : CommandBinder<TransformOneToMany.Data>
{
    public override Command PCommand { get; } = new("transform-one-to-many", "Transform a JSON Twitch file into many new files");

    private readonly Tokens tokens = new();

    protected override void AddTokens()
    {
        PCommand.Add(tokens.InputArgument);
        PCommand.Add(tokens.CsvArgument);
        PCommand.Add(tokens.StartOption);
        PCommand.Add(tokens.EndOption);
        PCommand.Add(tokens.FormatOption);
        PCommand.Add(tokens.OutputDirOption);
        PCommand.Add(tokens.QuietOption);
    }

    protected override Data GetBoundValue(BindingContext bindingContext)
    {
        T Arg<T>(Argument<T> argument) => GetArgValue(argument, bindingContext);
        T Opt<T>(Option<T> option) => GetOptValue(option, bindingContext);
        return new
        (
            Arg(tokens.InputArgument),
            Arg(tokens.CsvArgument),
            Opt(tokens.StartOption),
            Opt(tokens.EndOption),
            Opt(tokens.FormatOption),
            Opt(tokens.OutputDirOption),
            Opt(tokens.QuietOption)
        );
    }

    public readonly record struct Data
    (
        string InputPath,
        string CsvPath,
        long Start,
        long End,
        Format PFormat,
        string OutputDir,
        bool Quiet
    );

    protected override void Handle(Data data)
    {
        (string inputPath, string csvPath, long start, long end, Format format, string outputDir, bool quiet) = data;
        string input = File.ReadAllText(inputPath);
        JToken parent = (JToken)JsonConvert.DeserializeObject(input)!;
        CSVReader reader = CSVReader.FromFile(csvPath, CsvUtils.csvSettings);
        PrintLine("Writing files...", 0, quiet);
        foreach (TransformOneToManyCsv line in CsvSerialization.Deserialize<TransformOneToManyCsv>(reader))
        {
            if (line.outputFile == null)
            {
                PrintError("Output file must not be empty! Skipping...", 1);
                continue;
            }
            PrintLine(line.outputFile, 1, quiet);
            string outputFile = line.outputFile;
            start = line.start ?? start;
            end = line.end ?? end;
            format = line.format ?? format;
            outputDir = line.outputDir ?? outputDir;
            _ = Directory.CreateDirectory(outputDir);
            string outputPath = outputDir.EndsWith('\\') ? outputDir + outputFile : outputDir + '\\' + outputFile;
            string output;
            JToken parentClone = parent.DeepClone();
            try
            {
                output = TransformHandler.MTransform(parentClone, start, end, format);
            }
            catch (JsonReaderException e)
            {
                PrintError($"Could not parse JSON file {inputPath}", 2);
                PrintError(e.Message, 2);
                continue;
            }
            catch (Exception e)
            {
                PrintError($"JSON file {inputPath} parsed successfully but the contents were unexpected", 2);
                PrintError(e.Message, 2);
                continue;
            }
            File.WriteAllText(outputPath, output);
        }
    }
}