using System.CommandLine;

namespace TwitchChatOffset.Options;

public static class OptionUtils
{
    public static void AddOptions<TOptionGroup>(this Command command) where TOptionGroup : class, IOptionGroup<TOptionGroup>, new()
    {
        foreach (FieldData fieldData in IOptionGroup<TOptionGroup>.FieldDatas)
        {
            CliOptionAttribute attribute = (CliOptionAttribute)fieldData.Attribute;
            command.Add(attribute.Option);
        }
    }

    public static TOptionGroup ParseOptions<TOptionGroup>(this ParseResult parseResult) where TOptionGroup : class, IOptionGroup<TOptionGroup>, new()
    {
        TOptionGroup options = new();
        foreach (FieldData fieldData in IOptionGroup<TOptionGroup>.FieldDatas)
        {
            IPlicit value = fieldData.GetValue(parseResult);
            options.WriteField(fieldData, value);
        }
        return options;
    }
}