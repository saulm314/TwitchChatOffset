using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSVFile;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public static class TransformHandler
{
    public static void HandleTransform(string inputPath, string outputPath, long start, long end, Format format)
    {
        string input = File.ReadAllText(inputPath);
        JToken parent = (JToken)JsonConvert.DeserializeObject(input)!;
        HandleTransform(parent, outputPath, start, end, format);
    }

    public static void HandleTransform(JToken parent, string outputPath, long start, long end, Format format)
    {
        ApplyOffset(parent, start, end);
        string output = ApplyFormat(parent, format);
        File.WriteAllText(outputPath, output);
    }

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
        Tokens.AddAliasesToOptionMap(optionMap, pairs);
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
                HandleTransform(line.inputFile!, outputPath, (long)line.start!, (long)line.end!, (Format)line.format!);
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
            HandleTransform(clonedParent, outputPath, line.start, line.end, format);
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
                HandleTransform(fileName, outputPath, start, end, format);
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

    private static void ApplyOffset(JToken parent, long start, long end)
    {
        if (start == 0 && end == -1)
            return;
        JArray comments = (JArray)parent["comments"]!;
        int i = 0;
        while (i < comments.Count)
        {
            JToken comment = comments[i];
            JValue commentOffset = (JValue)comment["content_offset_seconds"]!;
            long commentOffsetValue = (long)commentOffset.Value!;
            if (commentOffsetValue < start)
            {
                comments.RemoveAt(i);
                continue;
            }
            if (end != -1 && commentOffsetValue > end)
            {
                comments.RemoveAt(i);
                continue;
            }
            commentOffset.Value = commentOffsetValue - start;
            i++;
        }
    }

    private static string ApplyFormat(JToken parent, Format format)
    {
        return format switch
        {
            Format.Json => ApplyFormatJson(parent),
            Format.JsonIndented => ApplyFormatJsonIndented(parent),
            Format.Plaintext => ApplyFormatPlaintext(parent),
            _ => throw new Exception("Internal error: unrecognised format type")
        };
    }

    private static string ApplyFormatJson(JToken parent)
    {
        return JsonConvert.SerializeObject(parent);
    }

    private static string ApplyFormatJsonIndented(JToken parent)
    {
        return JsonConvert.SerializeObject(parent, Formatting.Indented);
    }

    private static string ApplyFormatPlaintext(JToken parent)
    {
        StringBuilder stringBuilder = new();
        JArray comments = (JArray)parent["comments"]!;
        foreach (JToken comment in comments)
        {
            JValue commentOffset = (JValue)comment["content_offset_seconds"]!;
            long commentOffsetValue = (long)commentOffset.Value!;
            TimeSpan timeSpan = TimeSpan.FromSeconds(commentOffsetValue);
            stringBuilder.Append(timeSpan);
            stringBuilder.Append(' ');

            JValue displayName = (JValue)comment["commenter"]!["display_name"]!;
            string displayNameValue = (string)displayName.Value!;
            stringBuilder.Append(displayNameValue);
            stringBuilder.Append(": ");

            JValue messageBody = (JValue)comment["message"]!["body"]!;
            string messageBodyValue = (string)messageBody.Value!;
            stringBuilder.Append(messageBodyValue);
            stringBuilder.Append('\n');
        }
        return stringBuilder.ToString();
    }
}