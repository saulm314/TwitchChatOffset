using TwitchChatOffset.Json;
using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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