namespace TwitchChatOffset.CommandLine.Options;

public readonly struct AliasesContainer
{
    public AliasesContainer(string[] aliases)
    {
        this.aliases = aliases;
        strippedAliases = new string[aliases.Length];
        aliases.CopyTo(strippedAliases, 0);
        for (int i = 0; i < strippedAliases.Length; i++)
            strippedAliases[i] = strippedAliases[i].TrimStart('-');
    }

    public readonly string[] aliases;
    public readonly string[] strippedAliases;
}