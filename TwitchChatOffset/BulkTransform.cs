using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System;
using CSVFile;
using Newtonsoft.Json.Linq;
using System.Text;

namespace TwitchChatOffset;

public static class BulkTransform
{
    public static void HandleTransformManyToMany(string csvPath, CsvOptions csvOptions, bool quiet)
    {
        BulkTransform.quiet = quiet;
        IEnumerable<AliasesCFieldPair> pairs = GetAliasesCFieldPairs<TransformManyToManyCsv>();
        Dictionary<string, CField> optionMap = GetOptionMap(pairs);
        WriteLine("Writing files...", 0, quiet);
        IEnumerable<TransformManyToManyCsv> data = GetProcessedLines(csvPath, optionMap, csvOptions);
        WriteFiles(data);
    }

    /*public static void HandleTransformOneToMany(string inputPath, string csvPath, string outputDir, Format format, bool quiet)
    {
        _ = Directory.CreateDirectory(outputDir);
        IEnumerable<TransformOneToManyCsv> data = CSV.Deserialize<TransformOneToManyCsv>(File.ReadAllText(csvPath), csvSettings);
        string input = File.ReadAllText(inputPath);
        JToken parent = (JToken)JsonConvert.DeserializeObject(input)!;
        WriteLine("Writing files...", 0, quiet);
        foreach (TransformOneToManyCsv line in data)
        {
            WriteLine($"{line.outputFile}", 1, quiet);
            string outputPath = outputDir.EndsWith('\\') ? outputDir + line.outputFile : outputDir + '\\' + line.outputFile;
            JToken clonedParent = parent.DeepClone();
            Transform.HandleTransform(clonedParent, outputPath, line.start, line.end, format);
        }
    }*/

    public static void HandleTransformOneToMany(string inputPath, string csvPath, CsvOptions csvOptions, bool quiet)
    {
        BulkTransform.quiet = quiet;

    }

    private static IEnumerable<AliasesCFieldPair> GetAliasesCFieldPairs<TBulkTransformCsv>() where TBulkTransformCsv : BulkTransformCsv
    {
        if (typeof(TBulkTransformCsv) == typeof(TransformManyToManyCsv))
            yield return new(["--input-file", "--inputFile"],   CField.New(typeof(TransformManyToManyCsv).GetField(nameof(TransformManyToManyCsv.inputFile))!));
        yield return new(["--output-file", "--outputFile"],     CField.New(typeof(BulkTransformCsv).GetField(nameof(BulkTransformCsv.outputFile))!));
        yield return new(Tokens.StartOptionAliases,             CField.New(typeof(BulkTransformCsv).GetField(nameof(BulkTransformCsv.start))!));
        yield return new(Tokens.EndOptionAliases,               CField.New(typeof(BulkTransformCsv).GetField(nameof(BulkTransformCsv.end))!));
        yield return new(Tokens.FormatOptionAliases,            CField.New(typeof(BulkTransformCsv).GetField(nameof(BulkTransformCsv.format))!));
        yield return new(Tokens.OutputDirOptionAliases,         CField.New(typeof(BulkTransformCsv).GetField(nameof(BulkTransformCsv.outputDir))!));
    }

    private static Dictionary<string, CField> GetOptionMap(IEnumerable<AliasesCFieldPair> pairs)
    {
        Dictionary<string, CField> optionMap = [];
        foreach (AliasesCFieldPair pair in pairs)
            foreach (string alias in pair.Aliases)
                optionMap[alias] = pair.PCField;
        return optionMap;
    }

    private static IEnumerable<TransformManyToManyCsv> GetProcessedLines(string csvPath, Dictionary<string, CField> optionMap, CsvOptions csvOptions)
    {
        GetHeadersAndLines(csvPath, out string[] headers, out IEnumerable<string[]> lines);
        return GetProcessedLines(lines, headers, optionMap, csvOptions);
    }

    private static IEnumerable<BulkTransformCsv> GetProcessedLines(string csvPath, string inputFile, Func<BulkTransformCsv> newOptions,
        Dictionary<string, CField> optionMap, CsvOptions csvOptions)
    {
        GetHeadersAndLines(csvPath, out string[] headers, out IEnumerable<string[]> lines);
        return GetProcessedLines(lines, inputFile, newOptions, headers, optionMap, csvOptions);
    }

    private static void GetHeadersAndLines(string csvPath, out string[] headers, out IEnumerable<string[]> lines)
    {
        CSVReader reader = CSVReader.FromFile(csvPath, csvSettings);
        headers = GetDashedHeaders(reader);
        lines = reader.Lines();
    }

    private static string[] GetDashedHeaders(CSVReader reader)
    {
        string[] dashedHeaders = new string[reader.Headers.Length];
        for (int i = 0; i < dashedHeaders.Length; i++)
            dashedHeaders[i] = "--" + reader.Headers[i];
        return dashedHeaders;
    }

    private static IEnumerable<TransformManyToManyCsv> GetProcessedLines(IEnumerable<string[]> lines, string[] headers, Dictionary<string, CField> optionMap,
        CsvOptions csvOptions)
    {
        foreach (string[] line in lines)
        {
            TransformManyToManyCsv options = new();
            bool valid = GetProcessedLine(options, line, headers, optionMap, csvOptions);
            if (valid)
                yield return options;
        }
    }

    private static IEnumerable<BulkTransformCsv> GetProcessedLines(IEnumerable<string[]> lines, string inputFile, Func<BulkTransformCsv> newOptions,
        string[] headers, Dictionary<string, CField> optionMap, CsvOptions csvOptions)
    {
        foreach (string[] line in lines)
        {
            BulkTransformCsv options = newOptions();
            bool valid = GetProcessedLine(options, in inputFile, line, headers, optionMap, csvOptions);
            if (valid)
                yield return options;
        }
    }

    private static bool GetProcessedLine(TransformManyToManyCsv options, string[] line, string[] headers, Dictionary<string, CField> optionMap,
        CsvOptions csvOptions)
        => GetProcessedLine(options, in options.inputFile, line, headers, optionMap, csvOptions);

    // returns false if line is invalid
    private static bool GetProcessedLine(BulkTransformCsv options, in string? inputFile, string[] line, string[] headers, Dictionary<string, CField> optionMap,
        CsvOptions csvOptions)
    {
        for (int i = 0; i < line.Length; i++)
            WriteField(options, line[i], headers[i], optionMap);
        if (inputFile == null)
        {
            WriteError($"Input file must not be empty! Skipping...", 1);
            return false;
        }
        if (options.outputFile == null)
        {
            WriteError($"Output file must not be empty! Skipping...", 1);
            return false;
        }
        if (!quiet)
            WriteLine($"{options.outputFile}", 1);
        (long start, long end, Format format, string outputDir) = csvOptions;
        options.start ??= start;
        options.end ??= end;
        options.format ??= format;
        options.outputDir ??= outputDir;
        return true;
    }

    private static void WriteField(BulkTransformCsv options, string field, string header, Dictionary<string, CField> optionMap)
    {
        if (!optionMap.TryGetValue(header, out CField cField))
            return;
        if (field == string.Empty)
            return;
        if (!cField.Converter.IsValid(field))
        {
            WriteWarning($"Cannot convert \"{field}\" to type {cField.Field.FieldType.FullName}; treating as an empty string...", 1);
            return;
        }
        object? value = cField.Converter.ConvertFromString(field);
        cField.Field.SetValue(options, value);
    }

    private static void WriteFiles(IEnumerable<TransformManyToManyCsv> lines)
    {
        foreach (TransformManyToManyCsv line in lines)
            WriteFile(line, line.inputFile!);
        WriteLine("Done.", 0, quiet);
    }

    private static void WriteFiles(IEnumerable<BulkTransformCsv> lines, string inputFile)
    {
        foreach (BulkTransformCsv line in lines)
            WriteFile(line, inputFile);
    }

    private static void WriteFile(BulkTransformCsv line, string inputFile)
    {
        _ = Directory.CreateDirectory(line.outputDir!);
        string outputPath = line.outputDir!.EndsWith('\\') ? line.outputDir + line.outputFile : line.outputDir + '\\' + line.outputFile;
        try
        {
            Transform.HandleTransform(inputFile, outputPath, (long)line.start!, (long)line.end!, (Format)line.format!);
        }
        catch (JsonReaderException e)
        {
            WriteError($"Could not parse JSON file {inputFile}", 2);
            WriteError(e.Message, 2);
        }
        catch (Exception e)
        {
            WriteError($"JSON file {inputFile} parsed successfully but the contents were unexpected", 2);
            WriteError(e.Message, 2);
        }
    }

    public static void HandleTransformAll(string suffix, string inputDir, string searchPattern, string outputDir, Format format, bool quiet, long start, long end)
    {
        _ = Directory.CreateDirectory(outputDir);
        string[] fileNames = Directory.GetFiles(inputDir, searchPattern);
        WriteEnumerable(fileNames, "Input files found:", 0, quiet);
        WriteLine("Writing files...", 0, quiet);
        foreach (string fileName in fileNames)
        {
            string fileNameBody = Path.GetFileNameWithoutExtension(fileName);

            StringBuilder outputPathBuilder = new();
            outputPathBuilder.Append(outputDir);
            if (!outputDir.EndsWith('\\'))
                outputPathBuilder.Append('\\');
            outputPathBuilder.Append(fileNameBody);
            outputPathBuilder.Append(suffix);
            string outputPath = outputPathBuilder.ToString();
            WriteLine($"{outputPath}", 2, quiet);

            try
            {
                Transform.HandleTransform(fileName, outputPath, start, end, format);
            }
            catch (JsonReaderException e)
            {
                WriteError($"Could not parse JSON file {fileName}", 2);
                WriteError(e.Message, 2);
            }
            catch (Exception e)
            {
                WriteError($"JSON file {fileName} parsed successfully but the contents were unexpected", 2);
                WriteError(e.Message, 2);
            }
        }
    }

    private static readonly CSVSettings csvSettings = new()
    {
        FieldDelimiter = ','
    };

    private static bool quiet = false;
}