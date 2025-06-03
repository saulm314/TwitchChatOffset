using System;

namespace TwitchChatOffset;

// it is recommended that any class that extends from this one is either sealed or also abstract (and keeping NullablesGood and NullablesChecked as abstract)
// this is because a direct subclass is forced to implement NullablesGood and NullablesChecked since they are abstract,
//      on the other hand there is nothing forcing a subsubclass to do the same, so whoever builds the subsubclass could simply forget to override them
// so to prevent potential mistakes, all classes that extend from this one should either be sealed or abstract
//      and all abstract subclasses should have the NullablesGood and NullablesChecked members also marked as abstract
public abstract class BulkTransformCsv : ICsvData
{
    [Aliases(["output-file", "outputFile"])]
    public string? outputFile;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.Start))]
    public long? start;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.End))]
    public long? end;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.PFormat))]
    public Format? format;

    [Aliases(typeof(OptionAliases), nameof(OptionAliases.OutputDir))]
    public string? outputDir;

    public abstract string InputFile { get; }
    public string OutputFile => NullablesChecked ? outputFile! : throw new Exception("Internal error: cannot access OutputFile without checking nullables");
    public CsvOptions PCsvOptions 
        =>
            NullablesChecked ?
            new((long)start!, (long)end!, (Format)format!, outputDir!) :
            throw new Exception("Internal error: cannot access PCsvOptions without writing default options");

    protected bool NullablesGoodBulkTransformCsv(out string explanation)
    {
        if (outputFile == null)
        {
            explanation = "Output file must not be empty! Skipping...";
            return false;
        }
        explanation = string.Empty;
        return true;
    }

    public void WriteDefaultToEmptyFields(CsvOptions defaultOptions)
    {
        (long _start, long _end, Format _format, string _outputDir) = defaultOptions;
        start ??= _start;
        end ??= _end;
        format ??= _format;
        outputDir ??= _outputDir;
        _defaultOptionsWritten = true;
    }

    bool ICsvData.DefaultOptionsWritten => _defaultOptionsWritten;
    private bool _defaultOptionsWritten = false;

    public abstract bool NullablesGood(out string explanation);

    bool ICsvData.NullablesChecked => NullablesChecked;
    protected abstract bool NullablesChecked { get; }
}