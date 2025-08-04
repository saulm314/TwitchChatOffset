using TwitchChatOffset.CommandLine.Options;
using TwitchChatOffset.CSV;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public static class BulkTransform
{
    public static TransformManyToManyCsv? TryGetNonNullableLine(TransformManyToManyCsvNullables nullables,
        NullableOption<long> cliStart, NullableOption<long> cliEnd, NullableOption<Format> cliFormat, NullableOption<string> cliOutputDir,
        long cliOptionPriority)
    {
        if (nullables.inputFile == null)
        {
            PrintError("Input file must not be empty! Skipping...", 1);
            return null;
        }
        if (nullables.outputFile == null)
        {
            PrintError("Output file must not be empty! Skipping...", 1);
            return null;
        }
        OptionPriority optionPriority = GetOptionPriority(nullables.optionPriority, cliOptionPriority);
        return optionPriority switch
        {
            OptionPriority.CSV => new
                (
                    nullables.inputFile,
                    nullables.outputFile,
                    ResolveClashPrioritiseCsv(nullables.start, cliStart),
                    ResolveClashPrioritiseCsv(nullables.end, cliEnd),
                    ResolveClashPrioritiseCsv(nullables.format, cliFormat),
                    ResolveClashPrioritiseCsv(nullables.outputDir, cliOutputDir)
                ),
            OptionPriority.CLI => new
                (
                    nullables.inputFile,
                    nullables.outputFile,
                    ResolveClashPrioritiseCli(nullables.start, cliStart),
                    ResolveClashPrioritiseCli(nullables.end, cliEnd),
                    ResolveClashPrioritiseCli(nullables.format, cliFormat),
                    ResolveClashPrioritiseCli(nullables.outputDir, cliOutputDir)
                ),
            _ => throw new InternalException("Internal error: unrecognised option priority")
        };
    }

    public static TransformOneToManyCsv? TryGetNonNullableLine(TransformOneToManyCsvNullables nullables,
        NullableOption<long> cliStart, NullableOption<long> cliEnd, NullableOption<Format> cliFormat, NullableOption<string> cliOutputDir,
        long cliOptionPriority)
    {
        if (nullables.outputFile == null)
        {
            PrintError("Output file must not be empty! Skipping...", 1);
            return null;
        }
        OptionPriority optionPriority = GetOptionPriority(nullables.optionPriority, cliOptionPriority);
        return optionPriority switch
        {
            OptionPriority.CSV => new
                (
                    nullables.outputFile,
                    ResolveClashPrioritiseCsv(nullables.start, cliStart),
                    ResolveClashPrioritiseCsv(nullables.end, cliEnd),
                    ResolveClashPrioritiseCsv(nullables.format, cliFormat),
                    ResolveClashPrioritiseCsv(nullables.outputDir, cliOutputDir)
                ),
            OptionPriority.CLI => new
                (
                    nullables.outputFile,
                    ResolveClashPrioritiseCli(nullables.start, cliStart),
                    ResolveClashPrioritiseCli(nullables.end, cliEnd),
                    ResolveClashPrioritiseCli(nullables.format, cliFormat),
                    ResolveClashPrioritiseCli(nullables.outputDir, cliOutputDir)
                ),
            _ => throw new InternalException("Internal error: unrecognised option priority")
        };
    }

    public static string GetOutputPath(string outputDir, string outputFile)
        => outputDir.EndsWith('\\') ? outputDir + outputFile : outputDir + '\\' + outputFile;

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

    public static string? TryTransform(string inputFile, string input, long start, long end, Format format)
    {
        string output;
        try
        {
            output = Transform.MTransform(input, start, end, format);
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

    public static string? TryTransform(string inputFile, JToken input, long start, long end, Format format)
    {
        string output;
        try
        {
            output = Transform.MTransform(input, start, end, format);
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

    //_____________________________________________________________________

    public static OptionPriority GetOptionPriority(long? csvOptionPriority, long cliOptionPriority)
        => (csvOptionPriority ?? 0) >= cliOptionPriority ? OptionPriority.CSV : OptionPriority.CLI;

    // we duplicate these methods: one for where T : struct and one for where T : class
    // we could use where T : notnull to encapsulate both possibilities, but this feature appears to be bugged
    //      and doesn't actually allow structs
    //      specifically, T? gets interpreted as just T which for a struct is a compile-time error

    public static T ResolveClashPrioritiseCsv<T>(T? csvValue, INullableOption<T> cliOption) where T : struct
    {
        if (csvValue != null)
            return (T)csvValue;
        return cliOption.Value;
    }

    public static T ResolveClashPrioritiseCsv<T>(T? csvValue, INullableOption<T> cliOption) where T : class
    {
        if (csvValue != null)
            return csvValue;
        return cliOption.Value;
    }

    public static T ResolveClashPrioritiseCli<T>(T? csvValue, INullableOption<T> cliOption) where T : struct
    {
        if (cliOption.ValueSpecified)
            return cliOption.Value;
        if (csvValue != null)
            return (T)csvValue;
        return cliOption.Value;
    }

    public static T ResolveClashPrioritiseCli<T>(T? csvValue, INullableOption<T> cliOption) where T : class
    {
        if (cliOption.ValueSpecified)
            return cliOption.Value;
        if (csvValue != null)
            return csvValue;
        return cliOption.Value;
    }
}