using TwitchChatOffset.Json;
using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
using System;
using System.IO;
using Newtonsoft.Json;
using TwitchChatOffset.Options.Optimisations;

namespace TwitchChatOffset;

public static class BulkTransform
{
    public static TOptionGroup ResolveConflicts<TOptionGroup>(TOptionGroup csvOptions, TOptionGroup cliOptions) where TOptionGroup : class, IOptionGroup, new()
    {
        TOptionGroup options = new();
        FieldData[] fieldDatas = TOptionGroup.FieldDatas;
        foreach (FieldData fieldData in fieldDatas)
        {
            IPlicit csvPlicit = csvOptions.ReadField(fieldData);
            IPlicit cliPlicit = cliOptions.ReadField(fieldData);
            IPlicit winner = csvPlicit.Explicit ? csvPlicit : cliPlicit;
            options.WriteField(fieldData, winner);
        }
        return options;
    }

    public static Optimisation GetOptimisation(TransformManyCommonOptions? common0, TransformManyCommonOptions common1)
    {
        if (common0 == null)
            return Optimisation.None;
        string common0InputPath = Path.Combine(common0.InputDir, common0.InputFile);
        string common1InputPath = Path.Combine(common1.InputDir, common1.InputFile);
        if (common0InputPath != common1InputPath)
            return Optimisation.None;
        (long start0, long end0, long delay0) = common0.TransformOptions;
        (long start1, long end1, long delay1) = common1.TransformOptions;
        if (start0 != start1 || end0 != end1 || delay0 != delay1)
            return Optimisation.SameInputFile;
        if (common0.TransformOptions.Format != common1.TransformOptions.Format)
            return Optimisation.SameOffset;
        if (common0.TransformOptions.Format == Format.Ytt && common0.TransformOptions.SubtitleOptions != common1.TransformOptions.SubtitleOptions)
            return Optimisation.SameOffset;
        if (common0 != common1)
            return Optimisation.SameFormatSameSubtitleOptions;
        return Optimisation.Same;
    }

    public static bool TryGetSortedOriginalCommentsAndJson(TransformManyData data, string input, string inputPath)
    {
        data.MaxOptimisation = Optimisation.SameInputFile - 1;
        try
        {
            (data.OriginalComments, data.Json) = Transform.GetSortedOriginalCommentsAndJson(input);
        }
        catch (Exception e)
        {
            ProcessException(e, inputPath);
            PrintWarning($"Warning: skipping input file {inputPath}...", 2);
            data.SkipFile = true;
            return false;
        }
        data.MaxOptimisation = Optimisation.Same;
        return true;
    }

    public static bool TryApplyOffset(TransformManyData data, TransformCommonOptions options, string inputPath, string outputPath)
    {
        data.MaxOptimisation = Optimisation.SameOffset - 1;
        try
        {
            Transform.ApplyOffset(data.OriginalComments, data.Json, options);
        }
        catch (Exception e)
        {
            ProcessException(e, inputPath);
            PrintWarning($"Warning: skipping output file {outputPath}...", 2);
            return false;
        }
        data.MaxOptimisation = Optimisation.Same;
        return true;
    }

    public static bool TrySerialize(TransformManyData data, TransformCommonOptions options, string inputPath, string outputPath)
    {
        data.MaxOptimisation = Optimisation.SameFormatSameSubtitleOptions - 1;
        try
        {
            data.Output = Transform.Serialize(data.Json, options);
        }
        catch (Exception e)
        {
            ProcessException(e, inputPath);
            PrintWarning($"Warning: skipping output file {outputPath}...", 2);
            return false;
        }
        data.MaxOptimisation = Optimisation.Same;
        return true;
    }

    private static void ProcessException(Exception e, string inputPath)
    {
        switch (e)
        {
            case JsonException:
                PrintError($"Could not parse JSON file {inputPath}", 2);
                PrintError(e.Message, 2);
                break;
            case JsonContentException:
                PrintError($"JSON file {inputPath} parsed successfully but the contents were unexpected", 2);
                PrintError(e.Message, 2);
                break;
            case Exception:
                PrintError($"JSON file {inputPath} parsed successfully but an error occurred, possibly internal", 2);
                PrintError(e, 2);
                break;
        }
    }

    public static string? TryTransform(string inputFile, string input, TransformCommonOptions options)
    {
        string output;
        try
        {
            output = Transform.DoTransform(input, options);
        }
        catch (JsonException e)
        {
            PrintError($"Could not parse JSON file {inputFile}", 2);
            PrintError(e.Message, 2);
            return null;
        }
        catch (JsonContentException e)
        {
            PrintError($"JSON file {inputFile} parsed successfully but the contents were unexpected", 2);
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