using System;
using System.Collections.Generic;
using System.Reflection;

namespace TwitchChatOffset.Options;

public interface IOptionGroup<TOptionGroup> where TOptionGroup : class, IOptionGroup<TOptionGroup>, new()
{
    public static FieldData[] FieldDatas
    {
        get
        {
            if (_fieldDatas != null)
                return _fieldDatas;
            List<FieldData> fieldDatas = [];
            FieldInfo[] fields = typeof(TOptionGroup).GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo @field in fields)
            {
                AliasesAttribute? attribute = @field.GetCustomAttribute<AliasesAttribute>();
                if (attribute == null)
                {
                    Type iOptionGroupType = typeof(IOptionGroup<>).MakeGenericType(@field.FieldType);
                    if (!@field.FieldType.IsClass || !@field.FieldType.IsAssignableTo(iOptionGroupType))
                        continue;
                    PropertyInfo innerFieldDatasProperty = iOptionGroupType.GetProperty(nameof(FieldDatas), BindingFlags.Static | BindingFlags.Public)!;
                    FieldData[] innerFieldDatas = (FieldData[])innerFieldDatasProperty.GetMethod!.Invoke(null, [])!;
                    fieldDatas.AddRange(PrependField(innerFieldDatas, @field));
                    continue;
                }
                FieldData fieldData = new([@field], attribute);
                fieldDatas.Add(fieldData);
            }
            return _fieldDatas = [..fieldDatas];
        }
    }

    public void WriteField(FieldData fieldData, IPlicit value)
    {
        object o = this;
        for (int i = 0; i < fieldData.FieldPath.Length - 1; i++)
            o = fieldData.FieldPath[i].GetValue(o)!;
        fieldData.FieldPath[^1].SetValue(o, value);
    }

    public IPlicit ReadField(FieldData fieldData)
    {
        object o = this;
        foreach (FieldInfo field in fieldData.FieldPath)
            o = field.GetValue(o)!;
        return (IPlicit)o;
    }

    private static FieldData[]? _fieldDatas;

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