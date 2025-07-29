using System.Runtime.CompilerServices;

namespace TwitchChatOffset;

public static class HashUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode<T>(T value) where T : notnull => value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode<T>(T? nullable, int defaultHash) where T : struct => nullable.HasValue ? nullable.Value.GetHashCode() : defaultHash;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode<T>(T? nullable, int defaultHash) where T : class => nullable?.GetHashCode() ?? defaultHash;
}