using System.CommandLine;
using System.CommandLine.Binding;

namespace TwitchChatOffset.CommandLine.Commands;

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
}