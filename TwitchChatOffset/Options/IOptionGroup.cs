using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Reflection;

namespace TwitchChatOffset.Options;

public interface IOptionGroup
{
    public static void AddCliOptions<TOptionGroup>(Command command) where TOptionGroup : class, IOptionGroup
    {
        foreach (FieldData fieldData in TOptionGroup.FieldDatas)
        {
            CliOptionAttribute attribute = (CliOptionAttribute)fieldData.Attribute;
            command.Add(attribute.Option);
        }
    }

    public static TOptionGroup ParseOptions<TOptionGroup>(ParseResult parseResult) where TOptionGroup : class, IOptionGroup, new()
    {
        TOptionGroup options = new();
        foreach (FieldData fieldData in TOptionGroup.FieldDatas)
        {
            object value = fieldData.GetValue(parseResult);
            WriteField(options, fieldData, value);
        }
        return options;
    }

    public static void WriteField<TOptionGroup>(TOptionGroup obj, FieldData fieldData, object value) where TOptionGroup : class, IOptionGroup, new()
    {
        object o = obj;
        for (int i = 0; i < fieldData.FieldPath.Length - 1; i++)
            o = fieldData.FieldPath[i].GetValue(o)!;
        fieldData.FieldPath[^1].SetValue(o, value);
    }

    public static object ReadField<TOptionGroup>(TOptionGroup obj, FieldData fieldData) where TOptionGroup : class, IOptionGroup, new()
    {
        object o = obj;
        foreach (FieldInfo field in fieldData.FieldPath)
            o = field.GetValue(o)!;
        return o;
    }

    static abstract FieldData[] FieldDatas { get; }

    protected static FieldData[] GetFieldDatas(Type type)
    {
        List<FieldData> fieldDatas = [];
        FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        foreach (FieldInfo field in fields)
        {
            AliasesAttribute? attribute = field.GetCustomAttribute<AliasesAttribute>();
            if (attribute == null)
            {
                if (!field.FieldType.IsClass || !field.FieldType.IsAssignableTo(typeof(IOptionGroup)))
                    continue;
                PropertyInfo innerFieldDatasProperty = field.FieldType.GetProperty(nameof(FieldDatas), BindingFlags.Static | BindingFlags.Public)!;
                FieldData[] innerFieldDatas = (FieldData[])innerFieldDatasProperty.GetMethod!.Invoke(null, [])!;
                fieldDatas.AddRange(PrependField(innerFieldDatas, field));
                continue;
            }
            FieldData fieldData = new([field], attribute);
            fieldDatas.Add(fieldData);
        }
        return [..fieldDatas];
    }

    private static IEnumerable<FieldData> PrependField(FieldData[] fieldDatas, FieldInfo field)
    {
        foreach (FieldData fieldData in fieldDatas)
        {
            FieldInfo[] fieldPath = new FieldInfo[fieldData.FieldPath.Length + 1];
            fieldPath[0] = field;
            fieldData.FieldPath.CopyTo(fieldPath, 1);
            yield return new(fieldPath, fieldData.Attribute);
        }
    }
}