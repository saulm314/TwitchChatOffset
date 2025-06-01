using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System;
using CSVFile;
using Newtonsoft.Json.Linq;
using System.Text;

namespace TwitchChatOffset;

public static class BulkTransform
{
    public static void HandleTransformManyToMany(string csvPath, long start, long end, Format format, string outputDir, bool quiet)
    {
        Dictionary<string, CField> optionMap = new();
        AliasesCFieldPair[] pairs =
        [
            new(["input-file", "inputFile"],    CField.New(typeof(TransformManyToManyCsv).GetField(nameof(TransformManyToManyCsv.inputFile))!)),
            new(["output-file", "outputFile"],  CField.New(typeof(TransformManyToManyCsv).GetField(nameof(TransformManyToManyCsv.outputFile))!)),
            new(Tokens.StartOptionAliases,      CField.New(typeof(TransformManyToManyCsv).GetField(nameof(TransformManyToManyCsv.start))!)),
            new(Tokens.EndOptionAliases,        CField.New(typeof(TransformManyToManyCsv).GetField(nameof(TransformManyToManyCsv.end))!)),
            new(Tokens.FormatOptionAliases,     CField.New(typeof(TransformManyToManyCsv).GetField(nameof(TransformManyToManyCsv.format))!)),
            new(Tokens.OutputDirOptionAliases,  CField.New(typeof(TransformManyToManyCsv).GetField(nameof(TransformManyToManyCsv.outputDir))!))
        ];
        AddAliasesToOptionMap(optionMap, pairs);
        IEnumerable<TransformManyToManyCsv> data = GetProcessedLines(csvPath, optionMap, start, end, format, outputDir);
        WriteLine("Writing files...");
        foreach (TransformManyToManyCsv line in data)
        {
            if (!quiet)
                WriteLine($"{line.outputFile}", 1);
            _ = Directory.CreateDirectory(line.outputDir!);
            string outputPath = line.outputDir!.EndsWith('\\') ? line.outputDir + line.outputFile : line.outputDir + '\\' + line.outputFile;
            try
            {
                Transform.HandleTransform(line.inputFile!, outputPath, (long)line.start!, (long)line.end!, (Format)line.format!);
            }
            catch (JsonReaderException e)
            {
                WriteError($"Could not parse JSON file {line.inputFile}", 2);
                WriteError(e.Message, 2);
            }
            catch (Exception e)
            {
                WriteError($"JSON file {line.inputFile} parsed successfully but the contents were unexpected", 2);
                WriteError(e.Message, 2);
            }
        }
        WriteLine("Done.");
    }

    private static void AddAliasesToOptionMap(Dictionary<string, CField> optionMap, AliasesCFieldPair[] pairs)
    {
        foreach (AliasesCFieldPair pair in pairs)
            foreach (string alias in pair.Aliases)
                optionMap[alias] = pair.PCField;
    }

    private static IEnumerable<TransformManyToManyCsv> GetProcessedLines(string csvPath, Dictionary<string, CField> optionMap,
        long start, long end, Format format, string outputDir)
    {
        CSVReader reader = CSVReader.FromFile(csvPath, csvSettings);
        string[] dashedHeaders = new string[reader.Headers.Length];
        for (int i = 0; i < reader.Headers.Length; i++)
            dashedHeaders[i] = "--" + reader.Headers[i];
        IEnumerable<string[]> lines = reader.Lines();
        foreach (string[] line in lines)
        {
            TransformManyToManyCsv options = new();
            for (int i = 0; i < line.Length; i++)
            {
                string? key =
                    optionMap.ContainsKey(reader.Headers[i]) ? reader.Headers[i] :
                    optionMap.ContainsKey(dashedHeaders[i]) ? dashedHeaders[i] : null;
                if (key == null)
                    continue;
                CField cfield = optionMap[key];
                if (line[i] == string.Empty)
                    continue;
                if (!cfield.Converter.IsValid(line[i]))
                {
                    WriteWarning($"Cannot convert \"{line[i]}\" to type {cfield.Field.FieldType.FullName}; treating as an empty string...", 2);
                    continue;
                }
                object? value = cfield.Converter.ConvertFromString(line[i]);
                cfield.Field.SetValue(options, value);
            }
            if (options.inputFile == null)
            {
                WriteError($"Input file must not be empty! Skipping...", 2);
                continue;
            }
            if (options.outputFile == null)
            {
                WriteError($"Output file must not be empty! Skipping...", 2);
                continue;
            }
            options.start ??= start;
            options.end ??= end;
            options.format ??= format;
            options.outputDir ??= outputDir;
            yield return options;
        }
    }

    public static void HandleTransformOneToMany(string inputPath, string csvPath, string outputDir, Format format, bool quiet)
    {
        _ = Directory.CreateDirectory(outputDir);
        IEnumerable<TransformOneToManyCsv> data = CSV.Deserialize<TransformOneToManyCsv>(File.ReadAllText(csvPath), csvSettings);
        string input = File.ReadAllText(inputPath);
        JToken parent = (JToken)JsonConvert.DeserializeObject(input)!;
        WriteLine("Writing files...");
        foreach (TransformOneToManyCsv line in data)
        {
            if (!quiet)
                WriteLine($"{line.outputFile}", 1);
            string outputPath = outputDir.EndsWith('\\') ? outputDir + line.outputFile : outputDir + '\\' + line.outputFile;
            JToken clonedParent = parent.DeepClone();
            Transform.HandleTransform(clonedParent, outputPath, line.start, line.end, format);
        }
    }

    public static void HandleTransformAll(string suffix, string inputDir, string searchPattern, string outputDir, Format format, bool quiet, long start, long end)
    {
        _ = Directory.CreateDirectory(outputDir);
        string[] fileNames = Directory.GetFiles(inputDir, searchPattern);
        if (!quiet)
            WriteEnumerable(fileNames, "Input files found:");
        WriteLine("Writing files...");
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
            if (!quiet)
                WriteLine($"{outputPath}", 2);

            try
            {
                Transform.HandleTransform(fileName, outputPath, start, end, format);
            }
            catch (JsonReaderException e)
            {
                WriteError($"Could not parse JSON file {fileName}", 2);
                WriteError(e.Message, 2);
            }
            catch (Exception e)
            {
                WriteError($"JSON file {fileName} parsed successfully but the contents were unexpected", 2);
                WriteError(e.Message, 2);
            }
        }
    }

    private static readonly CSVSettings csvSettings = new()
    {
        FieldDelimiter = ','
    };
}