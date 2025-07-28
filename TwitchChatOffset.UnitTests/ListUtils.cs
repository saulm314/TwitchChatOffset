using System;
using System.Collections;
using System.Collections.Generic;

namespace TwitchChatOffsetUnitTests;

public static class ListUtils
{
    public static int Max(int int1, params int[] ints)
    {
        int max = int1;
        foreach (int i in ints)
            if (i > max)
                max = i;
        return max;
    }

    public static int MaxLength(ICollection collection1, params ICollection[] collections)
    {
        int max = collection1.Count;
        foreach (ICollection collection in collections)
            if (collection.Count > max)
                max = collection.Count;
        return max;
    }

    public static T IthOrLast<T>(this IList<T> list, int i) => i < list.Count ? list[i] : list[^1];

    public static IEnumerable<T> Transform<T>(this IEnumerable<T> enumerable, Action<T> transform)
    {
        foreach (T item in enumerable)
            transform(item);
        return enumerable;
    }
}