using TwitchChatOffset.Options.Groups;
using System.Collections.Generic;
using System.IO;

namespace TwitchChatOffset.Options.Optimisations;

public class TransformManyCommonOptionsComparer : Comparer<TransformManyCommonOptions>
{
    public override int Compare(TransformManyCommonOptions? x, TransformManyCommonOptions? y) => (int)Compare_(x, y);

    private static long Compare_(TransformManyCommonOptions? x, TransformManyCommonOptions? y)
    {
        long compare;
        string xInputPath = Path.Combine(x!.InputDir, x.InputFile);
        string yInputPath = Path.Combine(y!.InputDir, y.InputFile);
        compare = string.Compare(xInputPath, yInputPath);
        if (compare != 0)
            return compare;

        (long xStart, long xEnd, long xDelay) = x.TransformOptions;
        (long yStart, long yEnd, long yDelay) = y.TransformOptions;
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

        string xOutputNoExt = Path.GetFileNameWithoutExtension(x.OutputFile);
        string yOutputNoExt = Path.GetFileNameWithoutExtension(y.OutputFile);
        int xRemaining = xOutputNoExt.GetHashCode() ^ x.OutputDir.Value.GetHashCode() ^ x.Suffix.Value.GetHashCode();
        int yRemaining = yOutputNoExt.GetHashCode() ^ y.OutputDir.Value.GetHashCode() ^ y.Suffix.Value.GetHashCode();
        compare = xRemaining - yRemaining;
        return compare;
    }
}