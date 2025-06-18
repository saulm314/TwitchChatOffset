using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public static class BulkTransform
{
    public static TransformManyToManyCsv? TryGetNonNullableLine(TransformManyToManyCsvNullables nullables, long cliStart, long cliEnd, Format cliFormat, string cliOutputDir)
    {
        if (nullables.inputFile == null)
        {
            PrintError("Input file must not be empty! Skipping...", 1);
            return null;
        }
        if (nullables.outputFile == null)
        {
            PrintError("Output file must not be empty! Skipping...", 2);
            return null;
        }
        return new
        (
            nullables.inputFile,
            nullables.outputFile,
            nullables.start ?? cliStart,
            nullables.end ?? cliEnd,
            nullables.format ?? cliFormat,
            nullables.outputDir ?? cliOutputDir
        );
    }

    public static TransformOneToManyCsv? TryGetNonNullableLine(TransformOneToManyCsvNullables nullables, long cliStart, long cliEnd, Format cliFormat, string cliOutputDir)
    {
        if (nullables.outputFile == null)
        {
            PrintError("Output file must not be empty! Skipping...", 2);
            return null;
        }
        return new
        (
            nullables.outputFile,
            nullables.start ?? cliStart,
            nullables.end ?? cliEnd,
            nullables.format ?? cliFormat,
            nullables.outputDir ?? cliOutputDir
        );
    }

    public static string GetOutputPath(string outputDir, string outputFile)
        => outputDir.EndsWith('\\') ? outputDir + outputFile : outputDir + '\\' + outputFile;

    public static string GetOutputPath(string inputFile, string outputDir, string outputSuffix)
    {
        string inputFileNameBody = Path.GetFileNameWithoutExtension(inputFile);
        StringBuilder outputPathBuilder = new();
        outputPathBuilder.Append(outputDir);
        if (!outputDir.EndsWith('\\'))
            outputPathBuilder.Append('\\');
        outputPathBuilder.Append(inputFileNameBody);
        outputPathBuilder.Append(outputSuffix);
        return outputPathBuilder.ToString();
    }


    public static string? TryTransform(string inputFile, string input, long start, long end, Format format)
    {
        string output;
        try
        {
            output = Transform.MTransform(input, start, end, format);
        }
        catch (JsonReaderException e)
        {
            PrintError($"Could not parse JSON file {inputFile}", 2);
            PrintError(e.Message, 2);
            return null;
        }
        catch (Exception e)
        {
            PrintError($"JSON file {inputFile} parsed successfully but the contents were unexpected", 2);
            PrintError(e.Message, 2);
            return null;
        }
        return output;
    }

    public static string? TryTransform(string inputFile, JToken input, long start, long end, Format format)
    {
        string output;
        try
        {
            output = Transform.MTransform(input, start, end, format);
        }
        catch (JsonReaderException e)
        {
            PrintError($"Could not parse JSON file {inputFile}", 2);
            PrintError(e.Message, 2);
            return null;
        }
        catch (Exception e)
        {
            PrintError($"JSON file {inputFile} parsed successfully but the contents were unexpected", 2);
            PrintError(e.Message, 2);
            return null;
        }
        return output;
    }
}