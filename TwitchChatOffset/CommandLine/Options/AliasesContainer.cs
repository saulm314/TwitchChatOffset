namespace TwitchChatOffset.CommandLine.Options;

public readonly struct AliasesContainer
{
    public AliasesContainer(string[] aliases)
    {
        Aliases = aliases;
        StrippedAliases = new string[aliases.Length];
        aliases.CopyTo(StrippedAliases, 0);
        for (int i = 0; i < StrippedAliases.Length; i++)
            StrippedAliases[i] = StrippedAliases[i].TrimStart('-');
    }

    public readonly string[] Aliases;
    public readonly string[] StrippedAliases;
}