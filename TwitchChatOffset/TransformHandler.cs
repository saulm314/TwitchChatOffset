using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSVFile;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonFormatting = Newtonsoft.Json.Formatting;

namespace TwitchChatOffset;

public static class TransformHandler
{
    public static void HandleTransform(string inputPath, string outputPath, long start, long end, Formatting formatting)
    {
        string input = File.ReadAllText(inputPath);
        JToken parent = (JToken)JsonConvert.DeserializeObject(input)!;
        HandleTransform(parent, outputPath, start, end, formatting);
    }

    public static void HandleTransform(JToken parent, string outputPath, long start, long end, Formatting formatting)
    {
        ApplyOffset(parent, start, end);
        string output = ApplyFormatting(parent, formatting);
        File.WriteAllText(outputPath, output);
    }

    public static void HandleTransformManyToMany(string csvPath, string outputDir, Formatting formatting)
    {
        Directory.CreateDirectory(outputDir);
        IEnumerable<TransformManyToManyCsv> data = CSV.Deserialize<TransformManyToManyCsv>(File.ReadAllText(csvPath), csvSettings);
        foreach (TransformManyToManyCsv line in data)
        {
            string outputPath = outputDir.EndsWith('\\') ? outputDir + line.outputFile : outputDir + '\\' + line.outputFile;
            HandleTransform(line.inputFile, outputPath, line.start, line.end, formatting);
        }
    }

    public static void HandleTransformOneToMany(string inputPath, string csvPath, string outputDir, Formatting formatting)
    {
        Directory.CreateDirectory(outputDir);
        IEnumerable<TransformOneToManyCsv> data = CSV.Deserialize<TransformOneToManyCsv>(File.ReadAllText(csvPath), csvSettings);
        string input = File.ReadAllText(inputPath);
        JToken parent = (JToken)JsonConvert.DeserializeObject(input)!;
        foreach (TransformOneToManyCsv line in data)
        {
            string outputPath = outputDir.EndsWith('\\') ? outputDir + line.outputFile : outputDir + '\\' + line.outputFile;
            JToken clonedParent = parent.DeepClone();
            HandleTransform(clonedParent, outputPath, line.start, line.end, formatting);
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

    private static string ApplyFormatting(JToken parent, Formatting formatting)
    {
        return formatting switch
        {
            Formatting.Json => ApplyFormattingJson(parent),
            Formatting.JsonIndented => ApplyFormattingJsonIndented(parent),
            Formatting.Plaintext => ApplyFormattingPlaintext(parent),
            _ => throw new Exception("Internal error: unrecognised formatting type")
        };
    }

    private static string ApplyFormattingJson(JToken parent)
    {
        return JsonConvert.SerializeObject(parent);
    }

    private static string ApplyFormattingJsonIndented(JToken parent)
    {
        return JsonConvert.SerializeObject(parent, JsonFormatting.Indented);
    }

    private static string ApplyFormattingPlaintext(JToken parent)
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