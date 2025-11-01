using TwitchChatOffset.Csv;
using TwitchChatOffset.Json;
using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
using TwitchChatOffset.Ytt;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YTSubConverter.Shared;

namespace TwitchChatOffset;

public static class BulkTransform
{
    public static TConflictingOptionGroup ResolveConflicts<TConflictingOptionGroup>(TConflictingOptionGroup csvOptions, TConflictingOptionGroup cliOptions)
        where TConflictingOptionGroup : class, IConflictingOptionGroup, new()
    {
        TConflictingOptionGroup options = new();
        bool csvPriority = csvOptions.OptionPriority >= cliOptions.OptionPriority;
        FieldData[] fieldDatas = TConflictingOptionGroup.FieldDatas;
        foreach (FieldData fieldData in fieldDatas)
        {
            IPlicit csvPlicit = (IPlicit)IOptionGroup.ReadField(csvOptions, fieldData);
            IPlicit cliPlicit = (IPlicit)IOptionGroup.ReadField(cliOptions, fieldData);
            object winner = (csvPlicit.Explicit, cliPlicit.Explicit) switch
            {
                (false, false) => cliPlicit,
                (true, false) => csvPlicit,
                (false, true) => cliPlicit,
                (true, true) => csvPriority ? csvPlicit : cliPlicit
            };
            IOptionGroup.WriteField(options, fieldData, winner);
        }
        return options;
    }

    public static string GetCombinedPath(string directory, string fileName)
        => directory.EndsWith('\\') ? directory + fileName : directory + '\\' + fileName;

    public static string GetOutputPath(string inputFileName, string outputDir, string outputSuffix)
    {
        string inputFileNameBody = Path.GetFileNameWithoutExtension(inputFileName);
        StringBuilder outputPathBuilder = new();
        outputPathBuilder.Append(outputDir);
        if (!outputDir.EndsWith('\\'))
            outputPathBuilder.Append('\\');
        outputPathBuilder.Append(inputFileNameBody);
        outputPathBuilder.Append(outputSuffix);
        return outputPathBuilder.ToString();
    }

    public static string? TryTransform(string inputFile, string input, TransformOptions options)
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

    public static string? TryTransform(string inputFile, JToken input, TransformOptions options)
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