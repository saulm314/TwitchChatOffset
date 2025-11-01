using System;
using System.Collections.Generic;
using System.Reflection;

namespace TwitchChatOffset.Options;

public interface IOptionGroup
{
    public static void WriteField<TOptionGroup>(OptionGroupBox<TOptionGroup> obj, FieldData fieldData, object? value) where TOptionGroup : struct, IOptionGroup
    {
        TypedReference reference = TypedReference.MakeTypedReference(obj, fieldData.FieldPath);
        FieldInfo field = fieldData.FieldPath[^1];
        field.SetValueDirect(reference, value!);
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
                if (!field.FieldType.IsByRefLike || !field.FieldType.IsAssignableTo(typeof(IOptionGroup)))
                    continue;
                PropertyInfo innerFieldDatasProperty = field.FieldType.GetProperty(nameof(FieldDatas), BindingFlags.Static | BindingFlags.Public)!;
                FieldData[] innerFieldDatas = (FieldData[])innerFieldDatasProperty.GetMethod!.Invoke(null, [])!;
                fieldDatas.AddRange(PrependField(innerFieldDatas, field));
                continue;
            }
            FieldData fieldData = new([OptionGroupBox.GetValueField(field.FieldType), field], attribute);
            fieldDatas.Add(fieldData);
        }
        return [..fieldDatas];
    }

    private static IEnumerable<FieldData> PrependField(FieldData[] fieldDatas, FieldInfo field)
    {
        foreach (FieldData fieldData in fieldDatas)
        {
            FieldInfo[] fieldPath = new FieldInfo[fieldData.FieldPath.Length + 1];
            fieldPath[0] = OptionGroupBox.GetValueField(field.FieldType);
            fieldPath[1] = field;
            Array.Copy(fieldData.FieldPath, 1, fieldPath, 2, fieldData.FieldPath.Length - 1);
            yield return new(fieldPath, fieldData.Attribute);
        }
    }
}