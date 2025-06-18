using System.ComponentModel;
using System.Reflection;

namespace TwitchChatOffset.legacy;

public readonly record struct CField(FieldInfo Field, TypeConverter Converter)
{
    public static CField New(FieldInfo field) => new(field, TypeDescriptor.GetConverter(field.FieldType));
}