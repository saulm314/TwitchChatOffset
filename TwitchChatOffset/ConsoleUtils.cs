using System;
using System.Collections;
using System.Reflection;

namespace TwitchChatOffset;

public static class ConsoleUtils
{
    public static void WriteLine(object? message) => Console.WriteLine(message);

    public static void Write(object? message) => Console.Write(message);

    public static void WriteError(object? message)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteWarning(object? message)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteEnumerable(IEnumerable enumerable, string? heading = null, string? indent = null)
    {
        if (heading != null)
            WriteLine(indent + heading);
        foreach (object? item in enumerable)
            WriteLine($"{indent}\t{item}");
    }

    public static void WriteType(Type type, object? obj, string? heading = null, string? indent = null)
    {
        if (heading != null)
            WriteLine(indent + heading);
        foreach (FieldInfo field in type.GetFields())
            WriteLine($"{indent}\tField:\t{field.Name} = {field.GetValue(obj)}");
        foreach (PropertyInfo property in type.GetProperties())
            WriteLine($"{indent}\tProperty:\t{property.Name} = {property.GetValue(obj)}");
    }
}