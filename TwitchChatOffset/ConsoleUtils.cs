using System;
using System.Collections;
using System.Reflection;

namespace TwitchChatOffset;

public static class ConsoleUtils
{
    public static void WriteLine(object? message, byte indent = 0, bool quiet = false)
    {
        if (quiet)
            return;
        Span<char> indentChars = stackalloc char[indent];
        indentChars.Fill('\t');
        string indentStr = new(indentChars);
        Console.WriteLine(indentStr + message);
    }

    public static void Write(object? message, byte indent = 0, bool quiet = false)
    {
        if (quiet)
            return;
        Span<char> indentChars = stackalloc char[indent];
        indentChars.Fill('\t');
        string indentStr = new(indentChars);
        Console.Write(indentStr + message);
    }

    public static void WriteError(object? message, byte indent = 0, bool quiet = false)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        WriteLine(message, indent, quiet);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteWarning(object? message, byte indent = 0, bool quiet = false)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        WriteLine(message, indent, quiet);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteEnumerable(IEnumerable enumerable, string? heading = null, byte indent = 0, bool quiet = false)
    {
        if (heading != null)
            WriteLine(heading, indent, quiet);
        foreach (object? item in enumerable)
            WriteLine($"\t{item}", indent, quiet);
    }

    public static void WriteType(Type type, object? obj, string? heading = null, byte indent = 0, bool quiet = false)
    {
        if (heading != null)
            WriteLine(heading, indent, quiet);
        foreach (FieldInfo field in type.GetFields())
            WriteLine($"\tField:\t{field.Name} = {field.GetValue(obj)}", indent, quiet);
        foreach (PropertyInfo property in type.GetProperties())
            WriteLine($"\tProperty:\t{property.Name} = {property.GetValue(obj)}", indent, quiet);
    }

    public static void WriteObjectMembers(object obj, string? heading = null, byte indent = 0, bool quiet = false)
        => WriteType(obj.GetType(), obj, heading, indent, quiet);
}