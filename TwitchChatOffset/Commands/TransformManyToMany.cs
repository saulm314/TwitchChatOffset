using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using CSVFile;
using Newtonsoft.Json;
using TransformHandler = TwitchChatOffset.Transform;

namespace TwitchChatOffset.Commands;

public class TransformManyToMany : CommandBinder<TransformManyToMany.Data>
{
    public override Command PCommand { get; } = new("transform-many-to-many", "Transform many Json Twitch files and generate new files for each transformation");

    private readonly Tokens tokens = new();

    protected override void AddTokens()
    {
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
        string CsvPath,
        long Start,
        long End,
        Format PFormat,
        string OutputDir,
        bool Quiet
    );

    protected override void Handle(Data data)
    {
        (string csvPath, long start, long end, Format format, string outputDir, bool quiet) = data;
        CSVReader reader = CSVReader.FromFile(csvPath, CsvUtils.csvSettings);
        WriteLine("Writing files...", 0, quiet);
        foreach (TransformManyToManyCsv line in CsvSerialization.Deserialize<TransformManyToManyCsv>(reader))
        {
            if (line.inputFile == null)
            {
                WriteError("Input file must not be empty! Skipping...", 1);
                continue;
            }
            if (line.outputFile == null)
            {
                WriteError("Output file must not be empty! Skipping...", 1);
                continue;
            }
            WriteLine(line.outputFile, 1, quiet);
            string inputFile = line.inputFile;
            string outputFile = line.outputFile;
            start = line.start ?? start;
            end = line.end ?? end;
            format = line.format ?? format;
            outputDir = line.outputDir ?? outputDir;
            _ = Directory.CreateDirectory(outputDir);
            string outputPath = outputDir.EndsWith('\\') ? outputDir + outputFile : outputDir + '\\' + outputFile;
            string output;
            try
            {
                output = TransformHandler.MTransform(inputFile, start, end, format);
            }
            catch (JsonReaderException e)
            {
                WriteError($"Could not parse JSON file {inputFile}", 2);
                WriteError(e.Message, 2);
                continue;
            }
            catch (Exception e)
            {
                WriteError($"JSON file {inputFile} parsed successfully but the contents were unexpected", 2);
                WriteError(e.Message, 2);
                continue;
            }
            File.WriteAllText(outputPath, output);
        }
    }
}