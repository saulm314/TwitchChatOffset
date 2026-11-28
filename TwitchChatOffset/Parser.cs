using System;
using System.ComponentModel;

namespace TwitchChatOffset;

public static class Parser
{
    public static object ParseElement(string input, Type type, TypeConverter converter)
    {
        object? element = TryParseElement_(input, converter);
        if (element != null)
            return element;
        PrintError($"Cannot parse {input} into element of type {type.FullName}");
        throw new ParserException();
    }

    public static object ParseArray(string input, string delimiter, Type elementType, TypeConverter elementConverter, object defaultElement)
    {
        object? array = TryParseArray_(input, delimiter, elementType, elementConverter, defaultElement);
        if (array != null)
            return array;
        PrintError($"Cannot parse {input} into array of element-type {elementType.FullName}");
        throw new ParserException();
    }

    public static object Parse(string input, string delimiter, Type type, Type elementType, TypeConverter elementConverter, object defaultElement)
    {
        object? obj = TryParse_(input, delimiter, type, elementType, elementConverter, defaultElement);
        if (obj != null)
            return obj;
        string typeDescription = !type.IsArray ? "element of type" : "array of element-type";
        PrintError($"Cannot parse {input} into {typeDescription} {elementType.FullName}");
        throw new ParserException();
    }

    public static object? TryParseElement(string input, Type type, TypeConverter converter)
    {
        object? element = TryParseElement_(input, converter);
        if (element != null)
            return element;
        PrintWarning($"Cannot parse {input} into element of type {type.FullName}", 0, true);
        return null;
    }

    public static object? TryParseArray(string input, string delimiter, Type elementType, TypeConverter elementConverter, object defaultElement)
    {
        object? array = TryParseArray_(input, delimiter, elementType, elementConverter, defaultElement);
        if (array != null)
            return array;
        PrintWarning($"Cannot parse {input} into array of element-type {elementType.FullName}", 0, true);
        return null;
    }

    public static object? TryParse(string input, string delimiter, Type type, Type elementType, TypeConverter elementConverter, object defaultElement)
    {
        object? obj = TryParse_(input, delimiter, type, elementType, elementConverter, defaultElement);
        if (obj != null)
            return obj;
        string typeDescription = !type.IsArray ? "element of type" : "array of element-type";
        PrintWarning($"Cannot parse {input} into {typeDescription} {elementType.FullName}", 0, true);
        return null;
    }

    private static object? TryParseElement_(string input, TypeConverter converter)
    {
        // we do not bother with the converter.IsValid(input) call, for two reasons:
        // 1. this method is case sensitive, meaning it does not accept an enum if the case doesn't match exactly
        // 2. this method just does a try catch anyway, also catching all types of exception
        try
        {
            return converter.ConvertFromString(input);
        }
        catch
        {
            return null;
        }
    }

    private static object? TryParseArray_(string input, string delimiter, Type elementType, TypeConverter elementConverter, object defaultElement)
    {
        string[] unparsedArray = input.Split(delimiter);
        Array array = Array.CreateInstance(elementType, unparsedArray.Length);
        for (int i = 0; i < unparsedArray.Length; i++)
        {
            object? element;
            if (unparsedArray[i] == string.Empty)
                element = defaultElement;
            else
                element = TryParseElement(input, elementType, elementConverter);
            if (element == null)
                return element;
            array.SetValue(element, i);
        }
        return array;
    }

    private static object? TryParse_(string input, string delimiter, Type type, Type elementType, TypeConverter elementConverter, object defaultElement)
        => !type.IsArray ?
        TryParseElement_(input, elementConverter) :
        TryParseArray_(input, delimiter, elementType, elementConverter, defaultElement);
}