using System.CommandLine;

namespace TwitchChatOffset.CommandLine.Options;

public class OptionContainer<T>
{
    public OptionContainer(AliasesContainer aliasesContainer, T defaultValue, string description)
    {
        _defaultValue = defaultValue;
        Option = new(aliasesContainer.aliases, GetDefaultValue, description);
    }

    public Option<T> Option { get; init; }
    public bool Explicit { get; private set; } = true;

    public static implicit operator Option<T>(OptionContainer<T> container) => container.Option;

    private readonly T _defaultValue;

    private T GetDefaultValue()
    {
        Explicit = false;
        return _defaultValue;
    }
}