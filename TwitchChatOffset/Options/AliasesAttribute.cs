using TwitchChatOffset.Options.Groups;
using System;
using System.Reflection;

namespace TwitchChatOffset.Options;

[AttributeUsage(AttributeTargets.Field)]
public class AliasesAttribute : Attribute
{
    public AliasesAttribute(string[] aliases) => AliasesContainer = new(aliases);

    public AliasesAttribute(AliasesContainer aliasesContainer) => AliasesContainer = aliasesContainer;

    // pass the name of a property in the Aliases class, e.g. nameof(Aliases.Start)
    public AliasesAttribute(string propertyName)
    {
        PropertyInfo property = typeof(Aliases).GetProperty(propertyName)!;
        MethodInfo getMethod = property.GetMethod!;
        AliasesContainer = (AliasesContainer)getMethod.Invoke(null, [])!;
    }

    public AliasesAttribute(Type type, string propertyName)
    {
        PropertyInfo property = type.GetProperty(propertyName)!;
        MethodInfo getMethod = property.GetMethod!;
        AliasesContainer = (AliasesContainer)getMethod.Invoke(null, [])!;
    }

    public readonly AliasesContainer AliasesContainer;
}