using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CSVFile;

namespace TwitchChatOffset.Csv;

public static class CsvSerialization
{
    public static CSVReader CreateReader(string path, char fieldDelimiter) => CSVReader.FromFile(path, new CSVSettings() { FieldDelimiter = fieldDelimiter });

    // deserialize CSV content into fields in a class TOptionGroup that implements IOptionGroup
    // type TOptionGroup may not have any duplicate aliases (across multiple fields or in the same field), or an internal exception will be thrown
    // if no data for a field is found, then it is left with its default value and explicit is set to false, else explicit is set to true
    public static IEnumerable<TOptionGroup> Deserialize<TOptionGroup>(CSVReader reader, string cliCsvArrayDelimiter)
        where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        ThrowIfDuplicateAliases<TOptionGroup>();
        List<HeaderGroup> headerGroups = GetHeaderGroups<TOptionGroup>(reader.Headers);
        foreach (string[] line in reader.Lines())
            foreach (TOptionGroup data in ReadLine<TOptionGroup>(reader.Headers, headerGroups, line, cliCsvArrayDelimiter))
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
    private static IEnumerable<TOptionGroup> Deserialize_<TOptionGroup>(CSVReader reader, string cliCsvArrayDelimiter)
        where TOptionGroup : OptionGroup<TOptionGroup>, new()
        => Deserialize<TOptionGroup>(reader, cliCsvArrayDelimiter);

    private static void ThrowIfDuplicateAliases<TOptionGroup>() where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        HashSet<string> aliases = [];
        foreach (FieldData fieldData in OptionGroup<TOptionGroup>.FieldDatas)
        {
            foreach (string alias in fieldData.Attribute.AliasesContainer.StrippedAliases)
            {
                if (!aliases.Add(alias))
                    throw new CsvSerializationInternalException.DuplicateAlias<TOptionGroup>(alias);
            }
        }
    }

    private static List<HeaderGroup> GetHeaderGroups<TOptionGroup>(ReadOnlySpan<string> headers) where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        OptionGroup<TOptionGroup>.InitFieldDatas();
        List<HeaderGroup> headerGroups = [];
        foreach (Range range in headers.Split("/next"))
        {
            Dictionary<string, FieldData> dataMap = GetDataMap<TOptionGroup>(headers, range, out int? arrayDelimiterIndex);
            headerGroups.Add(new(range, dataMap, arrayDelimiterIndex));
        }
        return headerGroups;
    }

    private static Dictionary<string, FieldData> GetDataMap<TOptionGroup>(ReadOnlySpan<string> headers, Range range, out int? arrayDelimiterIndex)
        where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        Dictionary<string, FieldData> dataMap = [];
        HashSet<FieldData> addedFields = [];
        arrayDelimiterIndex = null;
    Headers:
        for (int i = range.Start.Value; i < range.End.Value; i++)
        {
            string header = headers[i];
            foreach (FieldData fieldData in OptionGroup<TOptionGroup>.FieldDatas)
            {
                foreach (string alias in fieldData.Attribute.AliasesContainer.StrippedAliases)
                {
                    if (alias != header)
                        continue;
                    if (addedFields.Contains(fieldData))
                        throw new CsvContentException.DuplicateOption(header);
                    dataMap.Add(header, fieldData);
                    addedFields.Add(fieldData);
                    if (Aliases.CsvArrayDelimiter.FieldData.Value == fieldData)
                        arrayDelimiterIndex = i;
                    continue Headers;
                }
            }
        }
        return dataMap;
    }

    private static IEnumerable<TOptionGroup> ReadLine<TOptionGroup>(string[] allHeaders, List<HeaderGroup> headerGroups, string[] wholeLine,
        string cliCsvArrayDelimiter)
        where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        TOptionGroup? previousData = null;
        string? previousArrayDelimiter = null;
        foreach (HeaderGroup headerGroup in headerGroups)
        {
            (Range range, Dictionary<string, FieldData> dataMap, int? arrayDelimiterIndex) = headerGroup;
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
            string arrayDelimiter = cliCsvArrayDelimiter;
            if (previousArrayDelimiter != null)
                arrayDelimiter = previousArrayDelimiter;
            if (arrayDelimiterIndex != null && line[(int)arrayDelimiterIndex] != string.Empty)
                arrayDelimiter = previousArrayDelimiter = line[(int)arrayDelimiterIndex];
            for (int i = 0; i < fieldCount; i++)
                ReadField(data, line[i], headers[i], dataMap, arrayDelimiter);
            previousData = data;
            yield return data;
        }
    }

    private static void ReadField<TOptionGroup>(TOptionGroup data, string field, string header, Dictionary<string, FieldData> dataMap, string arrayDelimiter)
        where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        if (!dataMap.TryGetValue(header, out FieldData? fieldData))
            return;
        if (field == string.Empty)
            return;

        object? rawValue = Parser.TryParse(field, arrayDelimiter, fieldData.Type, fieldData.ElementType, fieldData.ElementConverter, null);
        if (rawValue == null)
        {
            PrintWarning("Treating as an empty field...", 1, true);
            return;
        }

        ConstructorInfo plicitConstructor = fieldData.PlicitType.GetConstructor([fieldData.Type, typeof(bool)])!;
        IPlicit plicit = (IPlicit)plicitConstructor.Invoke([rawValue, true]);
        data.WriteField(fieldData, plicit);
    }
}