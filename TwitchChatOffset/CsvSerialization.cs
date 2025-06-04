using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using CSVFile;

namespace TwitchChatOffset;

public static class CsvSerialization
{
    public static IEnumerable<T> Deserialize<T>(CSVReader reader)
    {
        Dictionary<string, FieldInfoInfo> dataMap = GetDataMap<T>(reader.Headers);
        ConstructorInfo constructor = typeof(T).GetConstructor([])
            ?? throw new CsvSerializationException($"Cannot serialize to type {typeof(T)} as it does not have a constructor with zero parameters");
        foreach (string[] line in reader.Lines())
        {
            T data = (T)constructor.Invoke([]);
            for (int i = 0; i < line.Length; i++)
                WriteField(data, line[i], reader.Headers[i], dataMap);
            yield return data;
        }
    }

    private static Dictionary<string, FieldInfoInfo> GetDataMap<T>(string[] headers)
    {
        Dictionary<string, FieldInfoInfo> dataMap = [];
        foreach (string header in headers)
        {
            foreach (FieldInfoInfo fieldInfo in GetFieldInfos<T>())
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

    private static IEnumerable<FieldInfoInfo> GetFieldInfos<T>()
    {
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        foreach (FieldInfo field in fields)
        {
            AliasesAttribute? attribute = field.GetCustomAttribute<AliasesAttribute>();
            if (attribute == null)
                continue;
            FieldInfoInfo fieldInfo = new(field, attribute);
            yield return fieldInfo;
        }
    }

    private static void WriteField<T>(T data, string field, string header, Dictionary<string, FieldInfoInfo> dataMap)
    {
        if (!dataMap.TryGetValue(header, out FieldInfoInfo fieldInfo))
            return;
        if (field == string.Empty)
            return;
        if (!fieldInfo.converter.IsValid(field))
        {
            WriteWarning($"Cannot convert \"{field}\" to type {fieldInfo.field.FieldType}; treating as an empty field...", 1);
            return;
        }
        object? value = fieldInfo.converter.ConvertFromString(field);
        fieldInfo.field.SetValue(data, value);
    }

    private readonly struct FieldInfoInfo(FieldInfo field, AliasesAttribute attribute)
    {
        public readonly FieldInfo field = field;
        public readonly TypeConverter converter = TypeDescriptor.GetConverter(field.FieldType);
        public readonly AliasesAttribute attribute = attribute;
    }
}