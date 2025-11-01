using System;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace TwitchChatOffset.Options;

public readonly record struct CliOptionContainer(Option Option, AliasesContainer AliasesContainer)
{
    public static CliOptionContainer New<T>(string name, AliasesContainer aliasesContainer, string description, Func<ArgumentResult, T> defaultValueFactory)
    {
        Option<T> option = new(name, aliasesContainer.Aliases)
        {
            Description = description,
            DefaultValueFactory = defaultValueFactory
        };
        return new(option, aliasesContainer);
    }
}