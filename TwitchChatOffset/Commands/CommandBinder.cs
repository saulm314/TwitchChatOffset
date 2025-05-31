using System;
using System.CommandLine;
using System.CommandLine.Binding;

namespace TwitchChatOffset.Commands;

public abstract class CommandBinder<TData> : BinderBase<TData>
{
    public void Add(Command parentCommand)
    {
        parentCommand.Add(PCommand);
        PCommand.SetHandler(Handle, this);
    }

    public abstract Command PCommand { get; }

    protected abstract void Handle(TData data);

    private Func<Argument<T>, T> GetArgMethod<T>(BindingContext bindingContext) => bindingContext.ParseResult.GetValueForArgument;
    private Func<Option<T>, T> GetOptMethod<T>(BindingContext bindingContext) => bindingContext.ParseResult.GetValueForOption!;
    protected T GetArgValue<T>(Argument<T> argument, BindingContext bindingContext) => GetArgMethod<T>(bindingContext)(argument);
    protected T GetOptValue<T>(Option<T> option, BindingContext bindingContext) => GetOptMethod<T>(bindingContext)(option);
}