using TwitchChatOffset.Options.Groups;
using System;
using System.CommandLine;
using System.Reflection;

namespace TwitchChatOffset.Options;

[AttributeUsage(AttributeTargets.Field)]
public class CliOptionAttribute : AliasesAttribute
{
    // pass the name of a property in the CliOptions class, e.g. nameof(CliOptions.Start)
    public CliOptionAttribute(string propertyName) : this(GetCliOptionContainer(propertyName)) { }

    public readonly Option Option;

    private CliOptionAttribute(CliOptionContainer optionContainer) : base(optionContainer.AliasesContainer) => Option = optionContainer.Option;

    private static CliOptionContainer GetCliOptionContainer(string propertyName)
    {
        PropertyInfo property = typeof(CliOptions).GetProperty(propertyName)!;
        MethodInfo getMethod = property.GetMethod!;
        return (CliOptionContainer)getMethod.Invoke(null, [])!;
    }
}