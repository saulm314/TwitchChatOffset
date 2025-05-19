using System;
using System.Collections;

namespace TwitchChatOffset;

public static class ConsoleUtils
{
    public static void WriteError(object? message)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteWarning(object? message)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteEnumerable(IEnumerable enumerable, string? heading = null)
    {
        if (heading != null)
            Console.WriteLine(heading);
        foreach (object? item in enumerable)
            Console.WriteLine($"\t{item}");
    }
}