using System;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace TwitchChatOffset.Options;

public readonly record struct CliOptionContainer<T>(Option<T> Option, AliasesContainer AliasesContainer) : ICliOptionContainer
{
    public CliOptionContainer(string name, AliasesContainer aliasesContainer, string description, Func<ArgumentResult, T> defaultValueFactory)
        : this(MakeOption(name, aliasesContainer, description, defaultValueFactory), aliasesContainer) { }

    Option ICliOptionContainer.Option => Option;

    private static Option<T> MakeOption(string name, AliasesContainer aliasesContainer, string description, Func<ArgumentResult, T> defaultValueFactory)
        => new(name, aliasesContainer.Aliases)
        {
            Description = description,
            DefaultValueFactory = defaultValueFactory
        };
}