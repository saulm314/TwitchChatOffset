using TwitchChatOffset.CommandLine;
using System;
using System.CommandLine;
using System.ComponentModel;
using System.Reflection;

namespace TwitchChatOffset.Options;

public class FieldData
{
    public FieldData(FieldInfo[] fieldPath, AliasesAttribute attribute)
    {
        FieldPath = fieldPath;
        Attribute = attribute;
        PlicitType = fieldPath[^1].FieldType;
        Type = PlicitType.GenericTypeArguments[0];
        ElementType = Type;
        if (ElementType.IsArray)
            ElementType = ElementType.GetElementType()!;
        ElementConverter = TypeDescriptor.GetConverter(ElementType);
        attribute.AliasesContainer.FieldData.Value = this;
        if (attribute is not CliOptionAttribute cliAttribute)
            return;
        _parseResultGetValueMethod = ParseResultGetValueMethodDefinition.MakeGenericMethod(cliAttribute.Option.ValueType);
    }

    public readonly FieldInfo[] FieldPath;
    public readonly AliasesAttribute Attribute;
    public readonly Type PlicitType;
    public readonly Type Type; // possibly array
    public readonly Type ElementType; // not array
    public readonly TypeConverter ElementConverter;

    public IPlicit GetValue(ParseResult parseResult, string? cliDelimiter)
    {
        CliOptionAttribute attribute = (CliOptionAttribute)Attribute;
        object rawValue = GetCliOptionValue(parseResult, attribute.Option);
        if (Type.IsArray)
            rawValue = Parser.ParseArray((string)rawValue, cliDelimiter!, ElementType, ElementConverter, null);
        bool @explicit = !parseResult.GetResult(attribute.Option)!.Implicit;
        ConstructorInfo plicitConstructor = PlicitType.GetConstructor([Type, typeof(bool)])!;
        IPlicit plicit = (IPlicit)plicitConstructor.Invoke([rawValue, @explicit]);
        return plicit;
    }
    
    static FieldData()
    {
        MethodInfo[] parseResultMethods = typeof(ParseResult).GetMethods(BindingFlags.Instance | BindingFlags.Public);
        ParseResultGetValueMethodDefinition = Array.Find(parseResultMethods, IsGetValueOptionMethod)!;

        static bool IsGetValueOptionMethod(MethodInfo method)
        {
            if (method.Name != nameof(ParseResult.GetValue))
                return false;
            MethodInfo genericMethod = method.MakeGenericMethod(typeof(long));
            ParameterInfo[] parameters = genericMethod.GetParameters();
            if (parameters.Length != 1)
                return false;
            if (parameters[0].ParameterType != typeof(Option<long>))
                return false;
            return true;
        }
    }

    public static readonly MethodInfo ParseResultGetValueMethodDefinition;

    private readonly MethodInfo _parseResultGetValueMethod;

    private object GetCliOptionValue(ParseResult parseResult, Option option) => _parseResultGetValueMethod.Invoke(parseResult, [option])!;
}