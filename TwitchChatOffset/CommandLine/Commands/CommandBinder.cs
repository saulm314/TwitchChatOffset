using TwitchChatOffset.CommandLine.Options;
using System.CommandLine;
using System.CommandLine.Binding;

namespace TwitchChatOffset.CommandLine.Commands;

public abstract class CommandBinder<TData> : BinderBase<TData>
{
    public void Add(Command parentCommand)
    {
        parentCommand.Add(Command);
        AddTokens();
        Command.SetHandler(Handle, this);
    }

    public abstract Command Command { get; }

    protected abstract void AddTokens();

    protected abstract void Handle(TData data);

    protected static T Arg<T>(BindingContext bindingContext, Argument<T> argument) => bindingContext.ParseResult.GetValueForArgument(argument);

    protected static OptionValueContainer<T> Opt<T>(BindingContext bindingContext, OptionContainer<T> optionContainer)
    {
        T value = bindingContext.ParseResult.GetValueForOption(optionContainer.Option)!;
        return new(value, optionContainer.Explicit);
    }
}