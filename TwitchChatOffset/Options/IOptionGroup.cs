using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Reflection;

namespace TwitchChatOffset.Options;

public interface IOptionGroup
{
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