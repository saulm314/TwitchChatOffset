using System.CommandLine;
using System.CommandLine.Binding;

namespace TwitchChatOffset.CommandLine.Arguments;

public abstract class TCOArgumentBase<T>(string? name, string? description = null) : Argument<T>(name, description)
{
    public T GetValue(BindingContext bindingContext)
    {
        if (value != null)
            throw new InternalException("Internal error: cannot call GetValue more than once");
        T _value = bindingContext.ParseResult.GetValueForArgument(this);
        value = _value;
        return _value;
    }

    public T Value => value ?? throw new InternalException("Internal error: must call GetValue before attempting to get Value from property");
    private T? value;
}