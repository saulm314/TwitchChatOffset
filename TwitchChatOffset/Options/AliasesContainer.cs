namespace TwitchChatOffset.Options;

public readonly record struct AliasesContainer
{
    public AliasesContainer(string[] aliases)
    {
        Aliases = aliases;
        StrippedAliases = new string[aliases.Length];
        for (int i = 0; i < StrippedAliases.Length; i++)
            StrippedAliases[i] = aliases[i].TrimStart('-');
    }

    public readonly string[] Aliases;
    public readonly string[] StrippedAliases;
    public readonly Box<FieldData?> FieldData = new(null); // will break if multiple fieldDatas in the same execution point to the same AliasesContainer
}