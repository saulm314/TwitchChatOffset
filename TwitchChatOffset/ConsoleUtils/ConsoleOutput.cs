using System;
using System.Collections;
using System.Reflection;

namespace TwitchChatOffset.ConsoleUtils;

public static class ConsoleOutput
{
    public static void PrintLine(object? message, byte indent = 0, bool quiet = false)
    {
        if (quiet)
            return;
        Span<char> indentChars = stackalloc char[indent];
        indentChars.Fill('\t');
        string indentStr = new(indentChars);
        Console.WriteLine(indentStr + message);
    }

    public static void Print(object? message, byte indent = 0, bool quiet = false)
    {
        if (quiet)
            return;
        Span<char> indentChars = stackalloc char[indent];
        indentChars.Fill('\t');
        string indentStr = new(indentChars);
        Console.Write(indentStr + message);
    }

    public static void PrintError(object? message, byte indent = 0, bool quiet = false)
    {
        if (quiet)
            return;
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        PrintLine(message, indent);
        Console.ForegroundColor = originalColor;
    }

    public static void PrintWarning(object? message, byte indent = 0, bool quiet = false)
    {
        if (quiet)
            return;
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        PrintLine(message, indent);
        Console.ForegroundColor = originalColor;
    }

    public static void PrintEnumerable(IEnumerable enumerable, string? heading = null, byte indent = 0, bool quiet = false)
    {
        if (quiet)
            return;
        if (heading != null)
            PrintLine(heading, indent);
        foreach (object? item in enumerable)
            PrintLine($"\t{item}", (byte)(indent + 1));
    }

    public static void PrintType(Type type, object? obj, string? heading = null, byte indent = 0, bool quiet = false)
    {
        if (quiet)
            return;
        if (heading != null)
            PrintLine(heading, indent);
        BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic;
        bindingFlags = obj != null ? bindingFlags : bindingFlags | BindingFlags.Static;
        foreach (FieldInfo field in type.GetFields(bindingFlags))
        {
            if (field.IsStatic)
                PrintLine($"\tStatic Field:\t{field.Name} = {field.GetValue(null)}", (byte)(indent + 1));
            else
                PrintLine($"\tInstance Field:\t{field.Name} = {field.GetValue(obj)}", (byte)(indent + 1));
        }
        foreach (PropertyInfo property in type.GetProperties(bindingFlags))
        {
            if (!property.CanRead)
                continue;
            if (property.GetMethod!.IsStatic)
                PrintLine($"\tStatic Property: \t{property.Name} = {property.GetValue(null)}", (byte)(indent + 1));
            else
                PrintLine($"\tInstance Property:\t{property.Name} = {property.GetValue(obj)}", indent);
        }
    }

    public static void PrintObjectMembers(object obj, string? heading = null, byte indent = 0, bool quiet = false)
    {
        if (quiet)
            return;
        PrintType(obj.GetType(), obj, heading, indent);
    }
}