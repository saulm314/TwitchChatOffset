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
        Converter = TypeDescriptor.GetConverter(fieldPath[^1].FieldType);
        if (attribute is not CliOptionAttribute cliAttribute)
            return;
        _parseResultGetValueMethod = ParseResultGetValueMethodDefinition.MakeGenericMethod(cliAttribute.Option.ValueType);

    }

    public readonly FieldInfo[] FieldPath;
    public readonly AliasesAttribute Attribute;
    public readonly TypeConverter Converter;

    public IPlicit GetValue(ParseResult parseResult)
    {
        CliOptionAttribute attribute = (CliOptionAttribute)Attribute;
        object rawValue = _parseResultGetValueMethod!.Invoke(parseResult, [attribute.Option])!;
        bool @explicit = !parseResult.GetResult(attribute.Option)!.Implicit;
        Type plicitType = PlicitTypeDefinition.MakeGenericType(attribute.Option.ValueType);
        ConstructorInfo plicitConstructor = plicitType.GetConstructor([attribute.Option.ValueType, typeof(bool)])!;
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

    private readonly MethodInfo? _parseResultGetValueMethod;

    public static readonly Type PlicitTypeDefinition = typeof(Plicit<long>).GetGenericTypeDefinition();
}