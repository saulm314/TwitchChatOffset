using System.CommandLine;

namespace TwitchChatOffset.Options;

public interface ICliOptionContainer
{
    Option Option { get; }
    AliasesContainer AliasesContainer { get; }
}