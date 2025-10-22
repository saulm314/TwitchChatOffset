using TwitchChatOffset.CommandLine.Options;
using TwitchChatOffset.Csv;
using TwitchChatOffset.Json;
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
    public static TransformManyToManyCsv? TryGetNonNullableLine(TransformManyToManyCsvNullables nullables, ImplicitValue<long> cliStart,
        ImplicitValue<long> cliEnd, ImplicitValue<long> cliDelay, ImplicitValue<Format> cliFormat, ImplicitValue<AnchorPoint> cliYttPosition,
        ImplicitValue<long> cliYttMaxMessages, ImplicitValue<long> cliYttMaxCharsPerLine, ImplicitValue<double> cliYttScale, ImplicitValue<Shadow> cliYttShadow,
        ImplicitValue<long> cliYttBackgroundOpacity, ImplicitValue<string> cliYttTextColor, ImplicitValue<string> cliYttShadowColor, 
        ImplicitValue<string> cliYttBackgroundColor, ImplicitValue<string> cliOutputDir, long cliOptionPriority)
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
                    ResolveClashPrioritiseCsv(nullables.Delay, cliDelay),
                    ResolveClashPrioritiseCsv(nullables.Format, cliFormat),
                    ResolveClashPrioritiseCsv(nullables.YttPosition, cliYttPosition),
                    ResolveClashPrioritiseCsv(nullables.YttMaxMessages, cliYttMaxMessages),
                    ResolveClashPrioritiseCsv(nullables.YttMaxCharsPerLine, cliYttMaxCharsPerLine),
                    ResolveClashPrioritiseCsv(nullables.YttScale, cliYttScale),
                    ResolveClashPrioritiseCsv(nullables.YttShadow, cliYttShadow),
                    ResolveClashPrioritiseCsv(nullables.YttBackgroundOpacity, cliYttBackgroundOpacity),
                    ResolveClashPrioritiseCsv(nullables.YttTextColor, cliYttTextColor),
                    ResolveClashPrioritiseCsv(nullables.YttShadowColor, cliYttShadowColor),
                    ResolveClashPrioritiseCsv(nullables.YttBackgroundColor, cliYttBackgroundColor),
                    ResolveClashPrioritiseCsv(nullables.OutputDir, cliOutputDir)
                ),
            OptionPriority.CLI => new
                (
                    nullables.InputFile,
                    nullables.OutputFile,
                    ResolveClashPrioritiseCli(nullables.Start, cliStart),
                    ResolveClashPrioritiseCli(nullables.End, cliEnd),
                    ResolveClashPrioritiseCli(nullables.Delay, cliDelay),
                    ResolveClashPrioritiseCli(nullables.Format, cliFormat),
                    ResolveClashPrioritiseCli(nullables.YttPosition, cliYttPosition),
                    ResolveClashPrioritiseCli(nullables.YttMaxMessages, cliYttMaxMessages),
                    ResolveClashPrioritiseCli(nullables.YttMaxCharsPerLine, cliYttMaxCharsPerLine),
                    ResolveClashPrioritiseCli(nullables.YttScale, cliYttScale),
                    ResolveClashPrioritiseCli(nullables.YttShadow, cliYttShadow),
                    ResolveClashPrioritiseCli(nullables.YttBackgroundOpacity, cliYttBackgroundOpacity),
                    ResolveClashPrioritiseCli(nullables.YttTextColor, cliYttTextColor),
                    ResolveClashPrioritiseCli(nullables.YttShadowColor, cliYttShadowColor),
                    ResolveClashPrioritiseCli(nullables.YttBackgroundColor, cliYttBackgroundColor),
                    ResolveClashPrioritiseCli(nullables.OutputDir, cliOutputDir)
                ),
            _ => throw new InternalException("Internal error: unrecognised option priority")
        };
    }

    public static TransformOneToManyCsv? TryGetNonNullableLine(TransformOneToManyCsvNullables nullables, ImplicitValue<long> cliStart,
        ImplicitValue<long> cliEnd, ImplicitValue<long> cliDelay, ImplicitValue<Format> cliFormat, ImplicitValue<AnchorPoint> cliYttPosition,
        ImplicitValue<long> cliYttMaxMessages, ImplicitValue<long> cliYttMaxCharsPerLine, ImplicitValue<double> cliYttScale, ImplicitValue<Shadow> cliYttShadow,
        ImplicitValue<long> cliYttBackgroundOpacity, ImplicitValue<string> cliYttTextColor, ImplicitValue<string> cliYttShadowColor, 
        ImplicitValue<string> cliYttBackgroundColor, ImplicitValue<string> cliOutputDir, long cliOptionPriority)
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
                    ResolveClashPrioritiseCsv(nullables.Delay, cliDelay),
                    ResolveClashPrioritiseCsv(nullables.Format, cliFormat),
                    ResolveClashPrioritiseCsv(nullables.YttPosition, cliYttPosition),
                    ResolveClashPrioritiseCsv(nullables.YttMaxMessages, cliYttMaxMessages),
                    ResolveClashPrioritiseCsv(nullables.YttMaxCharsPerLine, cliYttMaxCharsPerLine),
                    ResolveClashPrioritiseCsv(nullables.YttScale, cliYttScale),
                    ResolveClashPrioritiseCsv(nullables.YttShadow, cliYttShadow),
                    ResolveClashPrioritiseCsv(nullables.YttBackgroundOpacity, cliYttBackgroundOpacity),
                    ResolveClashPrioritiseCsv(nullables.YttTextColor, cliYttTextColor),
                    ResolveClashPrioritiseCsv(nullables.YttShadowColor, cliYttShadowColor),
                    ResolveClashPrioritiseCsv(nullables.YttBackgroundColor, cliYttBackgroundColor),
                    ResolveClashPrioritiseCsv(nullables.OutputDir, cliOutputDir)
                ),
            OptionPriority.CLI => new
                (
                    nullables.OutputFile,
                    ResolveClashPrioritiseCli(nullables.Start, cliStart),
                    ResolveClashPrioritiseCli(nullables.End, cliEnd),
                    ResolveClashPrioritiseCli(nullables.Delay, cliDelay),
                    ResolveClashPrioritiseCli(nullables.Format, cliFormat),
                    ResolveClashPrioritiseCli(nullables.YttPosition, cliYttPosition),
                    ResolveClashPrioritiseCli(nullables.YttMaxMessages, cliYttMaxMessages),
                    ResolveClashPrioritiseCli(nullables.YttMaxCharsPerLine, cliYttMaxCharsPerLine),
                    ResolveClashPrioritiseCli(nullables.YttScale, cliYttScale),
                    ResolveClashPrioritiseCli(nullables.YttShadow, cliYttShadow),
                    ResolveClashPrioritiseCli(nullables.YttBackgroundOpacity, cliYttBackgroundOpacity),
                    ResolveClashPrioritiseCli(nullables.YttTextColor, cliYttTextColor),
                    ResolveClashPrioritiseCli(nullables.YttShadowColor, cliYttShadowColor),
                    ResolveClashPrioritiseCli(nullables.YttBackgroundColor, cliYttBackgroundColor),
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

    public static string? TryTransform(string inputFile, string input, long start, long end, long delay, Format format, AnchorPoint yttPosition,
        long yttMaxMessages, long yttMaxCharsPerLine, double yttScale, Shadow yttShadow, long yttBackgroundOpacity, string yttTextColor, string yttShadowColor,
        string yttBackgroundColor)
    {
        string output;
        try
        {
            output = Transform.DoTransform(input, start, end, delay, format, yttPosition, yttMaxMessages, yttMaxCharsPerLine, yttScale, yttShadow,
                yttBackgroundOpacity, yttTextColor, yttShadowColor, yttBackgroundColor);
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

    public static string? TryTransform(string inputFile, JToken input, long start, long end, long delay, Format format, AnchorPoint yttPosition,
        long yttMaxMessages, long yttMaxCharsPerLine, double yttScale, Shadow yttShadow, long yttBackgroundOpacity, string yttTextColor, string yttShadowColor,
        string yttBackgroundColor)
    {
        string output;
        try
        {
            output = Transform.DoTransform(input, start, end, delay, format, yttPosition, yttMaxMessages, yttMaxCharsPerLine, yttScale, yttShadow,
                yttBackgroundOpacity, yttTextColor, yttShadowColor, yttBackgroundColor);
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