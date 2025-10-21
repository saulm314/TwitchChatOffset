using TwitchChatOffset.CommandLine.Options;
using TwitchChatOffset.Csv;
using TwitchChatOffset.Json;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YTSubConverter.Shared;

namespace TwitchChatOffset;

public static class BulkTransform
{
    public static TransformManyToManyCsv? TryGetNonNullableLine(TransformManyToManyCsvNullables nullables, ImplicitValue<long> cliStart,
        ImplicitValue<long> cliEnd, ImplicitValue<Format> cliFormat, ImplicitValue<AnchorPoint> cliYttPosition, ImplicitValue<string> cliOutputDir,
        long cliOptionPriority)
    {
        if (nullables.InputFile == null)
        {
            PrintError("Input file must not be empty! Skipping...", 1);
            return null;
        }
        if (nullables.OutputFile == null)
        {
            PrintError("Output file must not be empty! Skipping...", 1);
            return null;
        }
        OptionPriority optionPriority = GetOptionPriority(nullables.OptionPriority, cliOptionPriority);
        return optionPriority switch
        {
            OptionPriority.CSV => new
                (
                    nullables.InputFile,
                    nullables.OutputFile,
                    ResolveClashPrioritiseCsv(nullables.Start, cliStart),
                    ResolveClashPrioritiseCsv(nullables.End, cliEnd),
                    ResolveClashPrioritiseCsv(nullables.Format, cliFormat),
                    ResolveClashPrioritiseCsv(nullables.YttPosition, cliYttPosition),
                    ResolveClashPrioritiseCsv(nullables.OutputDir, cliOutputDir)
                ),
            OptionPriority.CLI => new
                (
                    nullables.InputFile,
                    nullables.OutputFile,
                    ResolveClashPrioritiseCli(nullables.Start, cliStart),
                    ResolveClashPrioritiseCli(nullables.End, cliEnd),
                    ResolveClashPrioritiseCli(nullables.Format, cliFormat),
                    ResolveClashPrioritiseCli(nullables.YttPosition, cliYttPosition),
                    ResolveClashPrioritiseCli(nullables.OutputDir, cliOutputDir)
                ),
            _ => throw new InternalException("Internal error: unrecognised option priority")
        };
    }

    public static TransformOneToManyCsv? TryGetNonNullableLine(TransformOneToManyCsvNullables nullables, ImplicitValue<long> cliStart,
        ImplicitValue<long> cliEnd, ImplicitValue<Format> cliFormat, ImplicitValue<AnchorPoint> cliYttPosition, ImplicitValue<string> cliOutputDir,
        long cliOptionPriority)
    {
        if (nullables.OutputFile == null)
        {
            PrintError("Output file must not be empty! Skipping...", 1);
            return null;
        }
        OptionPriority optionPriority = GetOptionPriority(nullables.OptionPriority, cliOptionPriority);
        return optionPriority switch
        {
            OptionPriority.CSV => new
                (
                    nullables.OutputFile,
                    ResolveClashPrioritiseCsv(nullables.Start, cliStart),
                    ResolveClashPrioritiseCsv(nullables.End, cliEnd),
                    ResolveClashPrioritiseCsv(nullables.Format, cliFormat),
                    ResolveClashPrioritiseCsv(nullables.YttPosition, cliYttPosition),
                    ResolveClashPrioritiseCsv(nullables.OutputDir, cliOutputDir)
                ),
            OptionPriority.CLI => new
                (
                    nullables.OutputFile,
                    ResolveClashPrioritiseCli(nullables.Start, cliStart),
                    ResolveClashPrioritiseCli(nullables.End, cliEnd),
                    ResolveClashPrioritiseCli(nullables.Format, cliFormat),
                    ResolveClashPrioritiseCli(nullables.YttPosition, cliYttPosition),
                    ResolveClashPrioritiseCli(nullables.OutputDir, cliOutputDir)
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

    public static string? TryTransform(string inputFile, string input, long start, long end, Format format, AnchorPoint yttPosition)
    {
        string output;
        try
        {
            output = Transform.DoTransform(input, start, end, format, yttPosition);
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

    public static string? TryTransform(string inputFile, JToken input, long start, long end, Format format, AnchorPoint yttPosition)
    {
        string output;
        try
        {
            output = Transform.DoTransform(input, start, end, format, yttPosition);
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

    public static T ResolveClashPrioritiseCsv<T>(Wrap<T>? csvValue, ImplicitValue<T> cliValue) where T : notnull
        => csvValue == null ? cliValue : csvValue.Value.Value;

    public static T ResolveClashPrioritiseCli<T>(Wrap<T>? csvValue, ImplicitValue<T> cliValue) where T : notnull
        => !cliValue.Implicit || csvValue == null ? cliValue : csvValue.Value.Value;
}