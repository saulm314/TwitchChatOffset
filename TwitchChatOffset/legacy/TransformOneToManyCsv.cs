namespace TwitchChatOffset.legacy;

public sealed class TransformOneToManyCsv(string inputFile) : BulkTransformCsv, ICsvData
{
    public override string InputFile => inputFile;

    public override bool NullablesGood(out string explanation)
    {
        if (!NullablesGoodBulkTransformCsv(out explanation))
            return false;
        _nullablesChecked = true;
        return true;
    }

    protected override bool NullablesChecked => _nullablesChecked;
    private bool _nullablesChecked = false;
}