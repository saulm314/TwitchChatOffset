using System;
using System.Reflection;

namespace TwitchChatOffset;

[AttributeUsage(AttributeTargets.Field)]
public class AliasesAttribute : Attribute
{
    public AliasesAttribute(string[] aliases)
    {
        Aliases = aliases;
    }

    // pass the type and the name of the static property (with a public get accessor) within the type which stores the
    //      aliasesContainer so that they can be retrieved at runtime
    // for the propertyName, it is recommended to use (e.g.) nameof(OptionAliases.Start) rather than specifying "Start" directly
    public AliasesAttribute(Type type, string propertyName, bool stripped = true)
    {
        PropertyInfo property = type.GetProperty(propertyName)!;
        MethodInfo getMethod = property.GetMethod!;
        AliasesContainer container = (AliasesContainer)getMethod.Invoke(null, [])!;
        Aliases = stripped ? container.strippedAliases : container.aliases;
    }

    public string[] Aliases { get; init; }
}