using System;
using System.Collections;
using System.Reflection;

namespace TwitchChatOffset;

public static class ConsoleUtils
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
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        PrintLine(message, indent, quiet);
        Console.ForegroundColor = originalColor;
    }

    public static void PrintWarning(object? message, byte indent = 0, bool quiet = false)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        PrintLine(message, indent, quiet);
        Console.ForegroundColor = originalColor;
    }

    public static void PrintEnumerable(IEnumerable enumerable, string? heading = null, byte indent = 0, bool quiet = false)
    {
        if (heading != null)
            PrintLine(heading, indent, quiet);
        foreach (object? item in enumerable)
            PrintLine($"\t{item}", indent, quiet);
    }

    public static void PrintType(Type type, object? obj, string? heading = null, byte indent = 0, bool quiet = false)
    {
        if (heading != null)
            PrintLine(heading, indent, quiet);
        foreach (FieldInfo field in type.GetFields())
            PrintLine($"\tField:\t{field.Name} = {field.GetValue(obj)}", indent, quiet);
        foreach (PropertyInfo property in type.GetProperties())
            PrintLine($"\tProperty:\t{property.Name} = {property.GetValue(obj)}", indent, quiet);
    }

    public static void PrintObjectMembers(object obj, string? heading = null, byte indent = 0, bool quiet = false)
        => PrintType(obj.GetType(), obj, heading, indent, quiet);
}