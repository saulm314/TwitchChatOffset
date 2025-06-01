using System;
using System.Collections.Generic;
using System.Data;
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
        IEnumerable<TransformManyToManyCsv> data = CSV.Deserialize<TransformManyToManyCsv>(File.ReadAllText(csvPath), csvSettings);
        WriteLine("Writing files...");
        foreach (TransformManyToManyCsv line in data)
        {
            if (!quiet)
                WriteLine($"\t{line.outputFile}");
            line.start ??= start;
            line.end ??= end;
            line.format ??= format;
            line.outputDirectory = string.IsNullOrWhiteSpace(line.outputDirectory) ? outputDir : line.outputDirectory;
            _ = Directory.CreateDirectory(line.outputDirectory!);
            string outputPath = line.outputDirectory!.EndsWith('\\') ? line.outputDirectory + line.outputFile : line.outputDirectory + '\\' + line.outputFile;
            try
            {
                HandleTransform(line.inputFile, outputPath, (long)line.start, (long)line.end, (Format)line.format);
            }
            catch (JsonReaderException e)
            {
                WriteError($"Could not parse JSON file {line.inputFile}");
                WriteError(e.Message);
            }
            catch (Exception e)
            {
                WriteError($"JSON file {line.inputFile} parsed successfully but the contents were unexpected");
                WriteError(e.Message);
            }
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
                WriteLine($"\t{line.outputFile}");
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
                WriteLine($"\t{outputPath}");

            try
            {
                HandleTransform(fileName, outputPath, start, end, format);
            }
            catch (JsonReaderException e)
            {
                WriteError($"Could not parse JSON file {fileName}");
                WriteError(e.Message);
            }
            catch (Exception e)
            {
                WriteError($"JSON file {fileName} parsed successfully but the contents were unexpected");
                WriteError(e.Message);
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