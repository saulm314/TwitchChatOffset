using System;
using System.Collections;
using System.Reflection;

namespace TwitchChatOffset;

public static class ConsoleUtils
{
    public static void WriteLine(object? message, byte indent = 0)
    {
        Span<char> indentChars = stackalloc char[indent];
        indentChars.Fill('\t');
        string indentStr = new(indentChars);
        Console.WriteLine(indentStr + message);
    }

    public static void Write(object? message, byte indent = 0)
    {
        Span<char> indentChars = stackalloc char[indent];
        indentChars.Fill('\t');
        string indentStr = new(indentChars);
        Console.Write(indentStr + message);
    }

    public static void WriteError(object? message, byte indent = 0)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        WriteLine(message, indent);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteWarning(object? message, byte indent = 0)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        WriteLine(message, indent);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteEnumerable(IEnumerable enumerable, string? heading = null, byte indent = 0)
    {
        if (heading != null)
            WriteLine(heading, indent);
        foreach (object? item in enumerable)
            WriteLine($"\t{item}", indent);
    }

    public static void WriteType(Type type, object? obj, string? heading = null, byte indent = 0)
    {
        if (heading != null)
            WriteLine(heading, indent);
        foreach (FieldInfo field in type.GetFields())
            WriteLine($"\tField:\t{field.Name} = {field.GetValue(obj)}", indent);
        foreach (PropertyInfo property in type.GetProperties())
            WriteLine($"\tProperty:\t{property.Name} = {property.GetValue(obj)}", indent);
    }

    public static void WriteObjectMembers(object obj, string? heading = null, byte indent = 0)
        => WriteType(obj.GetType(), obj, heading, indent);
}