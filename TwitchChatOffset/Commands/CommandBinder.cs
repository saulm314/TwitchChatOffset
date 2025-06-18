using System;
using System.CommandLine;
using System.CommandLine.Binding;

namespace TwitchChatOffset.Commands;

public abstract class CommandBinder<TData> : BinderBase<TData>
{
    public void Add(Command parentCommand)
    {
        parentCommand.Add(PCommand);
        AddTokens();
        PCommand.SetHandler(Handle, this);
    }

    public abstract Command PCommand { get; }

    protected abstract void AddTokens();

    protected abstract void Handle(TData data);

    private static Func<Argument<T>, T> GetArgMethod<T>(BindingContext bindingContext) => bindingContext.ParseResult.GetValueForArgument;
    private static Func<Option<T>, T> GetOptMethod<T>(BindingContext bindingContext) => bindingContext.ParseResult.GetValueForOption!;
    protected static T GetArgValue<T>(Argument<T> argument, BindingContext bindingContext) => GetArgMethod<T>(bindingContext)(argument);
    protected static T GetOptValue<T>(Option<T> option, BindingContext bindingContext) => GetOptMethod<T>(bindingContext)(option);
}