using TwitchChatOffset.Options.Groups;
using System.CommandLine;

namespace TwitchChatOffset.Options;

public static class OptionUtils
{
    public static void AddOptions<TOptionGroup>(this Command command) where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        foreach (FieldData fieldData in OptionGroup<TOptionGroup>.FieldDatas)
        {
            CliOptionAttribute attribute = (CliOptionAttribute)fieldData.Attribute;
            command.Add(attribute.Option);
        }
    }

    public static TOptionGroup ParseOptions<TOptionGroup>(this ParseResult parseResult) where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        // we need to parse the CLI delimiter first, since parsing the other options may rely on knowing the CLI delimiter
        OptionGroup<TOptionGroup>.InitFieldDatas();
        string? cliDelimiter = (Plicit<string>?)CliOptions.CliDelimiter.AliasesContainer.FieldData.Value?.GetValue(parseResult, null);

        TOptionGroup options = new();
        foreach (FieldData fieldData in OptionGroup<TOptionGroup>.FieldDatas)
        {
            IPlicit value = fieldData.GetValue(parseResult, cliDelimiter);
            options.WriteField(fieldData, value);
        }
        return options;
    }
}