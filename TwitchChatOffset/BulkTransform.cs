using TwitchChatOffset.Json;
using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    public static TransformManyOptimisation GetOptimisation(TransformManyCsvOptions? csvOptions0, TransformManyCsvOptions csvOptions1)
    {
        if (csvOptions0 == null)
            return TransformManyOptimisation.None;
        TransformManyCommonOptions common0 = csvOptions0.CommonOptions;
        TransformManyCommonOptions common1 = csvOptions1.CommonOptions;
        if (common0.InputFile != common1.InputFile)
            return TransformManyOptimisation.None;
        if (common0.InputDir != common1.InputDir)
            return TransformManyOptimisation.None;
        (long start0, long end0, long delay0) = common0.TransformOptions;
        (long start1, long end1, long delay1) = common1.TransformOptions;
        if (start0 != start1 || end0 != end1 || delay0 != delay1)
            return TransformManyOptimisation.SameInputFile;
        if (common0.TransformOptions.Format != common1.TransformOptions.Format)
            return TransformManyOptimisation.SameOffset;
        if (common0.TransformOptions.SubtitleOptions != common1.TransformOptions.SubtitleOptions)
            return TransformManyOptimisation.SameFormat;
        if (csvOptions0 != csvOptions1)
            return TransformManyOptimisation.SameSubtitleOptions;
        return TransformManyOptimisation.Same;
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

    public static string? TryTransform(string inputFile, JToken input, TransformCommonOptions options)
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