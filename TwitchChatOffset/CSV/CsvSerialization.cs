using TwitchChatOffset.CommandLine.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using CSVFile;

namespace TwitchChatOffset.CSV;

public static class CsvSerialization
{
    // deserialize CSV content into fields in type T that have an AliasesAttribute
    // if no data for a field is found, then it is left with its default value
    public static IEnumerable<T> Deserialize<T>(CSVReader reader) where T : new()
    {
        Dictionary<string, FieldData> dataMap = GetDataMap<T>(reader.Headers);
        foreach (string[] line in reader.Lines())
        {
            T data = new();
            for (int i = 0; i < line.Length; i++)
                WriteField(data, line[i], reader.Headers[i], dataMap);
            yield return data;
        }
    }

    // this method calls the generic Deserialize<T> method but uses reflection to find the type
    // so the generic method is preferred as it is (slightly) faster and type-safe
    public static IEnumerable Deserialize(CSVReader reader, Type type)
    {
        return
            (IEnumerable)
            typeof(CsvSerialization)
            .GetMethod(nameof(DeserializeDummy))!
            .MakeGenericMethod(type)
            .Invoke(null, [reader])!;
    }

    private static IEnumerable<T> DeserializeDummy<T>(CSVReader reader) where T : new() => Deserialize<T>(reader);

    private static Dictionary<string, FieldData> GetDataMap<T>(string[] headers)
    {
        Dictionary<string, FieldData> dataMap = [];
        foreach (string header in headers)
        {
            foreach (FieldData fieldData in GetFieldDatas<T>())
            {
                foreach (string alias in fieldData.attribute.Aliases)
                {
                    if (alias == header)
                    {
                        if (dataMap.ContainsKey(header))
                            throw CsvContentException.DuplicateOption(header);
                        dataMap.Add(header, fieldData);
                        goto Found;
                    }
                }
            }
            Found:;
        }
        return dataMap;
    }

    private static IEnumerable<FieldData> GetFieldDatas<T>()
    {
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        foreach (FieldInfo field in fields)
        {
            AliasesAttribute? attribute = field.GetCustomAttribute<AliasesAttribute>();
            if (attribute == null)
                continue;
            FieldData fieldData = new(field, attribute);
            yield return fieldData;
        }
    }

    private static void WriteField<T>(T data, string field, string header, Dictionary<string, FieldData> dataMap)
    {
        if (!dataMap.TryGetValue(header, out FieldData fieldData))
            return;
        if (field == string.Empty)
            return;
        if (!fieldData.converter.IsValid(field))
        {
            PrintWarning($"Cannot convert \"{field}\" to type {fieldData.field.FieldType.FullName}; treating as an empty field...", 1);
            return;
        }
        object? value = fieldData.converter.ConvertFromString(field);
        fieldData.field.SetValue(data, value);
    }

    private readonly struct FieldData(FieldInfo field, AliasesAttribute attribute)
    {
        public readonly FieldInfo field = field;
        public readonly TypeConverter converter = TypeDescriptor.GetConverter(field.FieldType);
        public readonly AliasesAttribute attribute = attribute;
    }
}