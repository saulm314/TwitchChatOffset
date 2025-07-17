using System;
using System.CommandLine;
using System.CommandLine.Binding;

namespace TwitchChatOffset.CommandLine.Options;

public abstract class TCOOptionBase<TType>(string[] aliases, Func<TType> getDefaultValue, string? description = null)
    : Option<TType>(aliases, getDefaultValue, description)
{
    public TType GetValue(BindingContext bindingContext)
    {
        if (value != null)
            throw new InternalException("Internal error: cannot call GetValue more than once");
        TType _value = bindingContext.ParseResult.GetValueForOption(this)!;
        value = _value;
        return _value;
    }

    public TType Value => value ?? throw new InternalException("Internal error: must call GetValue before attempting to get Value from property");
    private TType? value;
}