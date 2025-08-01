using System;
using System.CommandLine;
using System.CommandLine.Binding;

namespace TwitchChatOffset.CommandLine.Options;

public abstract class TCOOptionBase<T>(string[] aliases, Func<T> getDefaultValue, string? description = null)
    : Option<T>(aliases, getDefaultValue, description)
{
    public T GetValue(BindingContext bindingContext)
    {
        if (value != null)
            throw new InternalException("Internal error: cannot call GetValue more than once");
        T _value = bindingContext.ParseResult.GetValueForOption(this)!;
        value = _value;
        return _value;
    }

    public T Value => value ?? throw new InternalException("Internal error: must call GetValue before attempting to get Value from property");
    private T? value;
}