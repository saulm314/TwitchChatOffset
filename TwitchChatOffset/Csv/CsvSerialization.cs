using TwitchChatOffset.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CSVFile;

namespace TwitchChatOffset.Csv;

public static class CsvSerialization
{
    // deserialize CSV content into fields in a class TOptionGroup that implements IOptionGroup
    // type TOptionGroup may not have any duplicate aliases (across multiple fields or in the same field), or an internal exception will be thrown
    // if no data for a field is found, then it is left with its default value and explicit is set to false, else explicit is set to true
    public static IEnumerable<TOptionGroup> Deserialize<TOptionGroup>(CSVReader reader) where TOptionGroup : class, IOptionGroup, new()
    {
        Dictionary<string, FieldData> dataMap = GetDataMap<TOptionGroup>(reader.Headers);
        foreach (string[] line in reader.Lines())
        {
            TOptionGroup data = new();
            int fieldCount = int.Min(line.Length, reader.Headers.Length);
            for (int i = 0; i < fieldCount; i++)
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
            .GetMethod(nameof(DeserializeDummy), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(type)
            .Invoke(null, [reader])!;
    }

    private static IEnumerable<TOptionGroup> DeserializeDummy<TOptionGroup>(CSVReader reader) where TOptionGroup : class, IOptionGroup, new()
        => Deserialize<TOptionGroup>(reader);

    private static Dictionary<string, FieldData> GetDataMap<TOptionGroup>(string[] headers) where TOptionGroup : class, IOptionGroup, new()
    {
        Dictionary<string, FieldData> dataMap = [];
        HashSet<FieldData> addedFields = [];
        FieldData[] fieldDatas = TOptionGroup.FieldDatas;
        ThrowIfDuplicateAliases<TOptionGroup>(fieldDatas);
        foreach (string header in headers)
        {
            foreach (FieldData fieldData in fieldDatas)
            {
                foreach (string alias in fieldData.Attribute.AliasesContainer.StrippedAliases)
                {
                    if (alias == header)
                    {
                        if (addedFields.Contains(fieldData))
                            throw new CsvContentException.DuplicateOption(header);
                        dataMap.Add(header, fieldData);
                        addedFields.Add(fieldData);
                        goto Found;
                    }
                }
            }
        Found:;
        }
        return dataMap;
    }

    private static void ThrowIfDuplicateAliases<TOptionGroup>(FieldData[] fieldDatas) where TOptionGroup : class, IOptionGroup, new()
    {
        HashSet<string> aliases = [];
        foreach (FieldData fieldData in fieldDatas)
        {
            foreach (string alias in fieldData.Attribute.AliasesContainer.StrippedAliases)
            {
                if (aliases.Contains(alias))
                    throw new CsvSerializationInternalException.DuplicateAlias<TOptionGroup>(alias);
                aliases.Add(alias);
            }
        }
    }

    private static void WriteField<TOptionGroup>(TOptionGroup data, string field, string header, Dictionary<string, FieldData> dataMap)
        where TOptionGroup : class, IOptionGroup, new()
    {
        if (!dataMap.TryGetValue(header, out FieldData? fieldData))
            return;
        if (field == string.Empty)
            return;

        // we do not bother with a fieldData.converter.IsValid(field) call, for two reasons:
        // 1. this method is case sensitive, meaning it does not accept an enum if the case doesn't match exactly
        // 2. this method just does a try catch anyway, also catching all types of exception
        object rawValue;
        try
        {
            rawValue = fieldData.Converter.ConvertFromString(field)!;
        }
        catch
        {
            PrintWarning($"Cannot convert \"{field}\" to type {fieldData.FieldPath[^1].FieldType.FullName}; treating as an empty field...", 1);
            return;
        }
        Type plicitType = FieldData.PlicitTypeDefinition.MakeGenericType(fieldData.FieldPath[^1].FieldType);
        ConstructorInfo plicitConstructor = plicitType.GetConstructor([fieldData.FieldPath[^1].FieldType, typeof(bool)])!;
        object plicit = plicitConstructor.Invoke([rawValue, true]);
        IOptionGroup.WriteField(data, fieldData, plicit);
    }
}