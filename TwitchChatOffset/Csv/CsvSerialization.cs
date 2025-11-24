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
    public static IEnumerable<TOptionGroup> Deserialize<TOptionGroup>(CSVReader reader) where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        ThrowIfDuplicateAliases<TOptionGroup>();
        List<HeaderGroup> headerGroups = GetHeaderGroups<TOptionGroup>(reader.Headers);
        foreach (string[] line in reader.Lines())
            foreach (TOptionGroup data in WriteLine<TOptionGroup>(reader.Headers, headerGroups, line))
                yield return data;
    }

    // this method calls the generic Deserialize<T> method but uses reflection to find the type
    // so the generic method is preferred as it is (slightly) faster and type-safe
    public static IEnumerable Deserialize(CSVReader reader, Type type)
    {
        return
            (IEnumerable)
            _deserializeDummyMethod
            .MakeGenericMethod(type)
            .Invoke(null, [reader])!;
    }

    private static readonly MethodInfo _deserializeDummyMethod =
        typeof(CsvSerialization).GetMethod(nameof(Deserialize_), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static IEnumerable<TOptionGroup> Deserialize_<TOptionGroup>(CSVReader reader) where TOptionGroup : OptionGroup<TOptionGroup>, new()
        => Deserialize<TOptionGroup>(reader);

    private static List<HeaderGroup> GetHeaderGroups<TOptionGroup>(ReadOnlySpan<string> headers) where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        List<HeaderGroup> headerGroups = [];
        foreach (Range range in headers.Split("/next"))
        {
            ReadOnlySpan<string> headerGroup = headers[range];
            Dictionary<string, FieldData> dataMap = GetDataMap<TOptionGroup>(headerGroup);
            headerGroups.Add(new(range, dataMap));
        }
        return headerGroups;
    }

    private static Dictionary<string, FieldData> GetDataMap<TOptionGroup>(ReadOnlySpan<string> headers) where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        Dictionary<string, FieldData> dataMap = [];
        HashSet<FieldData> addedFields = [];
        foreach (string header in headers)
        {
            foreach (FieldData fieldData in OptionGroup<TOptionGroup>.FieldDatas)
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

    private static void ThrowIfDuplicateAliases<TOptionGroup>() where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        HashSet<string> aliases = [];
        foreach (FieldData fieldData in OptionGroup<TOptionGroup>.FieldDatas)
        {
            foreach (string alias in fieldData.Attribute.AliasesContainer.StrippedAliases)
            {
                if (aliases.Contains(alias))
                    throw new CsvSerializationInternalException.DuplicateAlias<TOptionGroup>(alias);
                aliases.Add(alias);
            }
        }
    }

    private static IEnumerable<TOptionGroup> WriteLine<TOptionGroup>(string[] allHeaders, List<HeaderGroup> headerGroups, string[] wholeLine)
        where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        TOptionGroup? previousData = null;
        foreach (HeaderGroup headerGroup in headerGroups)
        {
            (Range range, Dictionary<string, FieldData> dataMap) = headerGroup;
            TOptionGroup data = previousData?.DeepClone() ?? new();
            ReadOnlySpan<string> headers = allHeaders.AsSpan()[range];
            ReadOnlySpan<string> line;
            if (range.Start.Value >= wholeLine.Length)
                line = [];
            else if (range.End.Value >= wholeLine.Length)
                line = wholeLine.AsSpan()[range.Start..];
            else
                line = wholeLine.AsSpan()[range];
            int fieldCount = int.Min(line.Length, headers.Length);
            for (int i = 0; i < fieldCount; i++)
                WriteField(data, line[i], headers[i], dataMap);
            previousData = data;
            yield return data;
        }
    }

    private static void WriteField<TOptionGroup>(TOptionGroup data, string field, string header, Dictionary<string, FieldData> dataMap)
        where TOptionGroup : OptionGroup<TOptionGroup>, new()
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
            PrintWarning($"Cannot convert \"{field}\" to type {fieldData.FieldPath[^1].FieldType.GenericTypeArguments[0].FullName}; treating as an empty field...", 1);
            return;
        }
        Type plicitType = fieldData.FieldPath[^1].FieldType;
        ConstructorInfo plicitConstructor = plicitType.GetConstructor([fieldData.FieldPath[^1].FieldType.GenericTypeArguments[0], typeof(bool)])!;
        IPlicit plicit = (IPlicit)plicitConstructor.Invoke([rawValue, true]);
        data.WriteField(fieldData, plicit);
    }
}