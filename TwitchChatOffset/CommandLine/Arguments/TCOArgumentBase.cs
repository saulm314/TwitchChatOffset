using System.CommandLine;
using System.CommandLine.Binding;

namespace TwitchChatOffset.CommandLine.Arguments;

public abstract class TCOArgumentBase<TType>(string? name, string? description = null)
    : Argument<TType>(name, description)
{
    public TType GetValue(BindingContext bindingContext)
    {
        if (value != null)
            throw new InternalException("Internal error: cannot call GetValue more than once");
        TType _value = bindingContext.ParseResult.GetValueForArgument(this);
        value = _value;
        return _value;
    }

    public TType Value => value ?? throw new InternalException("Internal error: must call GetValue before attempting to get Value from property");
    private TType? value;
}