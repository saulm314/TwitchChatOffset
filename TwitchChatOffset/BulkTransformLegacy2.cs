using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using CSVFile;
using Newtonsoft.Json;

namespace TwitchChatOffset;

public static class BulkTransformLegacy2
{
    public static void HandleTransformManyToMany(string csvPath, CsvOptions csvOptions, bool quiet)
    {
        BulkTransformLegacy2.quiet = quiet;
        IEnumerable<TransformManyToManyCsv> data = GetProcessedLines(csvPath, csvOptions, () => new TransformManyToManyCsv());
        WriteFiles(data);
    }

    private static IEnumerable<TCsvData> GetProcessedLines<TCsvData>(string csvPath, CsvOptions csvOptions, Func<TCsvData> newData) where TCsvData : ICsvData
    {
        CSVReader reader = CSVReader.FromFile(csvPath, csvSettings);
        Dictionary<string, FieldInfoInfo> dataMap = GetDataMap<TCsvData>(reader.Headers);
        foreach (string[] line in reader.Lines())
        {
            TCsvData data = newData();
            bool valid = GetProcessedLine(data, line, reader.Headers, dataMap, csvOptions, out string explanation);
            if (!valid)
                WriteError(explanation, 1);
            yield return data;
        }
    }

    private static Dictionary<string, FieldInfoInfo> GetDataMap<TCsvData>(string[] headers) where TCsvData : ICsvData
    {
        Dictionary<string, FieldInfoInfo> dataMap = [];
        List<FieldInfoInfo> fieldInfos = GetFieldInfos<TCsvData>();
        foreach (string header in headers)
        {
            foreach (FieldInfoInfo fieldInfo in fieldInfos)
            {
                foreach (string alias in fieldInfo.attribute.Aliases)
                {
                    if (header == alias)
                    {
                        dataMap.Add(alias, fieldInfo);
                        goto Found;
                    }
                }
            }
            Found:;
        }
        return dataMap;
    }

    private static List<FieldInfoInfo> GetFieldInfos<TCsvData>() where TCsvData : ICsvData
    {
        FieldInfo[] fields = typeof(TCsvData).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        List<FieldInfoInfo> fieldInfos = new(fields.Length);
        foreach (FieldInfo field in fields)
        {
            AliasesAttribute? attribute = FieldInfoInfo.GetAliasesAttribute(field);
            if (attribute == null)
                continue;
            FieldInfoInfo fieldInfo = new(field, attribute);
            fieldInfos.Add(fieldInfo);
        }
        return fieldInfos;
    }

    private static bool GetProcessedLine(ICsvData data, string[] line, string[] headers, Dictionary<string, FieldInfoInfo> dataMap, CsvOptions csvOptions,
        out string explanation)
    {
        for (int i = 0; i < line.Length; i++)
            WriteField(data, line[i], headers[i], dataMap);
        if (!data.NullablesGood(out explanation))
            return false;
        WriteLine($"{data.OutputFile}", 1, quiet);
        data.WriteDefaultToEmptyFields(csvOptions);
        return true;
    }

    private static void WriteField(ICsvData data, string field, string header, Dictionary<string, FieldInfoInfo> dataMap)
    {
        if (!dataMap.TryGetValue(header, out FieldInfoInfo fieldInfo))
            return;
        if (field == string.Empty)
            return;
        if (!fieldInfo.converter.IsValid(field))
        {
            WriteWarning($"Cannot convert \"{field}\" to type {fieldInfo.field.FieldType.FullName}; treating as an empty string...", 1);
            return;
        }
        object? value = fieldInfo.converter.ConvertFromString(field);
        fieldInfo.field.SetValue(data, value);
    }

    private static void WriteFiles<TCsvData>(IEnumerable<TCsvData> lines) where TCsvData : ICsvData
    {
        foreach (TCsvData line in lines)
        {
            (string inputFile, string outputFile, long start, long end, Format format, string outputDir) = line;
            _ = Directory.CreateDirectory(outputDir);
            string outputPath = outputDir.EndsWith('\\') ? outputDir + outputFile : outputDir + '\\' + outputFile;
            try
            {
                Transform.HandleTransform(inputFile, outputPath, start, end, format);
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
        WriteLine("Done.", 0, quiet);
    }

    private static readonly CSVSettings csvSettings = new()
    {
        FieldDelimiter = ','
    };

    private static bool quiet = false;

    private readonly struct FieldInfoInfo(FieldInfo field, AliasesAttribute attribute)
    {
        public readonly FieldInfo field = field;
        public readonly TypeConverter converter = TypeDescriptor.GetConverter(field.FieldType);
        public readonly AliasesAttribute attribute = attribute;

        public static AliasesAttribute? GetAliasesAttribute(FieldInfo field) => field.GetCustomAttribute<AliasesAttribute>();
    }
}