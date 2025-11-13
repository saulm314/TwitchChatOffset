using TwitchChatOffset.Options.Groups;
using System.Collections.Generic;
using System.IO;

namespace TwitchChatOffset.Options.Comparers;

public class TransformManyCsvOptionsComparer : Comparer<TransformManyCsvOptions>
{
    public override int Compare(TransformManyCsvOptions? x, TransformManyCsvOptions? y) => (int)Compare_(x, y);

    private static long Compare_(TransformManyCsvOptions? x, TransformManyCsvOptions? y)
    {
        switch (x, y)
        {
            case (null, null):
                return 0;
            case (not null, null):
                return 1;
            case (null, not null):
                return -1;
            default:
                break;
        }
        long compare;
        string xInputPath = Path.Combine(x.CommonOptions.InputDir, x.CommonOptions.InputFile);
        string yInputPath = Path.Combine(y.CommonOptions.InputDir, y.CommonOptions.InputFile);
        compare = string.Compare(xInputPath, yInputPath);
        if (compare != 0)
            return compare;
        (long xStart, long xEnd, long xDelay) = x.CommonOptions.TransformOptions;
        (long yStart, long yEnd, long yDelay) = y.CommonOptions.TransformOptions;
        compare = xStart - yStart;
        if (compare != 0)
            return compare;
        compare = xEnd - yEnd;
        if (compare != 0)
            return compare;
        compare = xDelay - yDelay;
        if (compare != 0)
            return compare;
        compare = x.CommonOptions.TransformOptions.Format.Value - y.CommonOptions.TransformOptions.Format.Value;
        if (compare != 0)
            return compare;
        compare = x.CommonOptions.TransformOptions.SubtitleOptions.GetHashCode() - y.CommonOptions.TransformOptions.SubtitleOptions.GetHashCode();
        if (compare != 0)
            return compare;
        string xOutputNoExt = Path.GetFileNameWithoutExtension(x.CommonOptions.OutputFile);
        string yOutputNoExt = Path.GetFileNameWithoutExtension(y.CommonOptions.OutputFile);
        int xRemaining = xOutputNoExt.GetHashCode() ^ x.CommonOptions.OutputDir.Value.GetHashCode() ^ x.CommonOptions.Suffix.Value.GetHashCode();
        int yRemaining = yOutputNoExt.GetHashCode() ^ y.CommonOptions.OutputDir.Value.GetHashCode() ^ y.CommonOptions.Suffix.Value.GetHashCode();
        compare = xRemaining - yRemaining;
        return compare;
    }
}