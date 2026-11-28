using System;
using System.ComponentModel;

namespace TwitchChatOffset;

public static class ArrayParser
{
    public static bool TryParseArray(string input, string delimiter, Type elementType, TypeConverter converter, out Array? output)
    {
        output = null;
        string[] unparsedArray = input.Split(delimiter);
        Array array = Array.CreateInstance(elementType, unparsedArray.Length);
        for (int i = 0; i < unparsedArray.Length; i++)
        {
            try
            {
                array.SetValue(converter.ConvertFromString(unparsedArray[i]), i);
            }
            catch
            {
                PrintWarning($"Cannot convert \"{unparsedArray[i]}\" to type {elementType.FullName}...", 1, true);
                return false;
            }
        }
        output = array;
        return true;
    }
}