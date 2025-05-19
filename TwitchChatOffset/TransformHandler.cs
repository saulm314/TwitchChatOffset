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

    public static void HandleTransformManyToMany(string csvPath, string outputDir, Format format, bool quiet)
    {
        Directory.CreateDirectory(outputDir);
        IEnumerable<TransformManyToManyCsv> data = CSV.Deserialize<TransformManyToManyCsv>(File.ReadAllText(csvPath), csvSettings);
        Console.WriteLine("Writing files...");
        foreach (TransformManyToManyCsv line in data)
        {
            string outputPath = outputDir.EndsWith('\\') ? outputDir + line.outputFile : outputDir + '\\' + line.outputFile;
            HandleTransform(line.inputFile, outputPath, line.start, line.end, format);
            if (!quiet)
                Console.WriteLine(line.outputFile);
        }
    }

    public static void HandleTransformOneToMany(string inputPath, string csvPath, string outputDir, Format format, bool quiet)
    {
        Directory.CreateDirectory(outputDir);
        IEnumerable<TransformOneToManyCsv> data = CSV.Deserialize<TransformOneToManyCsv>(File.ReadAllText(csvPath), csvSettings);
        string input = File.ReadAllText(inputPath);
        JToken parent = (JToken)JsonConvert.DeserializeObject(input)!;
        Console.WriteLine("Writing files...");
        foreach (TransformOneToManyCsv line in data)
        {
            string outputPath = outputDir.EndsWith('\\') ? outputDir + line.outputFile : outputDir + '\\' + line.outputFile;
            JToken clonedParent = parent.DeepClone();
            HandleTransform(clonedParent, outputPath, line.start, line.end, format);
            if (!quiet)
                Console.WriteLine(line.outputFile);
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