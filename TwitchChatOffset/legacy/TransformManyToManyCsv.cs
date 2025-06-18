using System;

namespace TwitchChatOffset.legacy;

public sealed class TransformManyToManyCsv : BulkTransformCsv, ICsvData
{
    [Aliases(["input-file", "inputFile"])]
    public string? inputFile;

    public override string InputFile => NullablesChecked ? inputFile! : throw new Exception("Internal error: cannot access InputFile without checking nullables");

    public override bool NullablesGood(out string explanation)
    {
        if (inputFile == null)
        {
            explanation = "Input file must not be empty! Skipping...";
            return false;
        }
        if (!NullablesGoodBulkTransformCsv(out explanation))
            return false;
        _nullablesChecked = true;
        return true;
    }

    protected override bool NullablesChecked => _nullablesChecked;
    private bool _nullablesChecked = false;
}