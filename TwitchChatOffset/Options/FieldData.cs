using System.ComponentModel;
using System.Reflection;

namespace TwitchChatOffset.Options;

public readonly record struct FieldData(FieldInfo[] FieldPath, AliasesAttribute Attribute)
{
    public readonly TypeConverter Converter = TypeDescriptor.GetConverter(FieldPath[^1].FieldType);
}