using System.Collections.Generic;
using System.IO;
using TwitchChatOffset.Options.Groups;

namespace TwitchChatOffset.Options.Optimisations;

public class TransformAllCommonOptionsComparer : Comparer<TransformAllCommonOptions>
{
    public static readonly TransformAllCommonOptionsComparer Instance = new();

    public override int Compare(TransformAllCommonOptions? x, TransformAllCommonOptions? y) => (int)Compare_(x, y);

    private static long Compare_(TransformAllCommonOptions? x, TransformAllCommonOptions? y)
    {
        long compare;
        (long xStart, long xEnd, long xDelay) = x!.TransformOptions;
        (long yStart, long yEnd, long yDelay) = y!.TransformOptions;
        compare = xStart - yStart;
        if (compare != 0)
            return compare;

        compare = xEnd - yEnd;
        if (compare != 0)
            return compare;

        compare = xDelay - yDelay;
        if (compare != 0)
            return compare;

        compare = x.TransformOptions.Format.Value - y.TransformOptions.Format.Value;
        if (compare != 0)
            return compare;

        compare = x.TransformOptions.SubtitleOptions.GetHashCode() - y.TransformOptions.SubtitleOptions.GetHashCode();
        if (compare != 0)
            return compare;

        int xRemaining = x.OutputDir.Value.GetHashCode() ^ x.Suffix.Value.GetHashCode();
        int yRemaining = y.OutputDir.Value.GetHashCode() ^ y.Suffix.Value.GetHashCode();
        compare = xRemaining - yRemaining;
        return compare;
    }
}