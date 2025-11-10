using System.CommandLine;
using System.Reflection;

namespace TwitchChatOffset.Options;

public static class OptionUtils
{
    public static void AddOptions<TOptionGroup>(this Command command) where TOptionGroup : class, IOptionGroup, new()
    {
        foreach (FieldData fieldData in TOptionGroup.FieldDatas)
        {
            CliOptionAttribute attribute = (CliOptionAttribute)fieldData.Attribute;
            command.Add(attribute.Option);
        }
    }

    public static TOptionGroup ParseOptions<TOptionGroup>(this ParseResult parseResult) where TOptionGroup : class, IOptionGroup, new()
    {
        TOptionGroup options = new();
        foreach (FieldData fieldData in TOptionGroup.FieldDatas)
        {
            IPlicit value = fieldData.GetValue(parseResult);
            options.WriteField(fieldData, value);
        }
        return options;
    }

    public static void WriteField<TOptionGroup>(this TOptionGroup obj, FieldData fieldData, IPlicit value) where TOptionGroup : class, IOptionGroup, new()
    {
        object o = obj;
        for (int i = 0; i < fieldData.FieldPath.Length - 1; i++)
            o = fieldData.FieldPath[i].GetValue(o)!;
        fieldData.FieldPath[^1].SetValue(o, value);
    }

    public static IPlicit ReadField<TOptionGroup>(this TOptionGroup obj, FieldData fieldData) where TOptionGroup : class, IOptionGroup, new()
    {
        object o = obj;
        foreach (FieldInfo field in fieldData.FieldPath)
            o = field.GetValue(o)!;
        return (IPlicit)o;
    }
}