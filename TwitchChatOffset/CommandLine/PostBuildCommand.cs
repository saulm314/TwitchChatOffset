using TwitchChatOffset.Csv;
using TwitchChatOffset.Options;
using TwitchChatOffset.Options.Groups;
using System.CommandLine;
using System.IO;
using System.Linq;
using CSVFile;

namespace TwitchChatOffset.CommandLine;

public static class PostBuildCommand
{
    public static readonly Command Command = new("post-build", "[Developer only] tasks that run immediately after the app builds") { Hidden = true };

    static PostBuildCommand()
    {
        Command.SetAction(Execute);
    }

    private static void Execute(ParseResult parseResult)
    {
        GenerateHelpMenus();
        CheckCsvTemplates();
    }

    private static void GenerateHelpMenus()
    {
        if (Directory.Exists("../help"))
            Directory.Delete("../help", true);
        Directory.CreateDirectory("../help");
        GenerateHelpMenus(RootCommand.Command);
    }

    private static void GenerateHelpMenus(Command command)
    {
        PrintLine($"Generating help menu for command {command.Name}...");
        StringWriter stringWriter = new();
        command.Parse("--help").Invoke(new() { Output = stringWriter } );
        string helpText = stringWriter.ToString();
        File.WriteAllText($"../help/{command.Name}.txt", helpText);

        foreach (Command subcommand in command.Subcommands.Where(command => !command.Hidden))
            GenerateHelpMenus(subcommand);
    }

    private static void CheckCsvTemplates()
    {
        CheckCsvTemplate<TransformManyCommonOptions>("transform-many");
        CheckCsvTemplate<TransformAllCommonOptions>("transform-all");
    }

    private static void CheckCsvTemplate<TOptionGroup>(string templateName) where TOptionGroup : OptionGroup<TOptionGroup>, new()
    {
        PrintLine($"Checking CSV template {templateName}.csv...");
        using CSVReader reader = CSVReader.FromFile($"../templates/{templateName}.csv", CsvUtils.CsvSettings);
        TOptionGroup options = CsvSerialization.Deserialize<TOptionGroup>(reader).First();
        foreach (FieldData fieldData in OptionGroup<TOptionGroup>.FieldDatas)
        {
            IPlicit plicit = options.ReadField(fieldData);
            if (plicit.Explicit)
                continue;
            PrintEnumerableError(fieldData.Attribute.AliasesContainer.StrippedAliases,
                $"{templateName} template error: the option with the following aliases is not specified:");
            throw new InternalException($"{templateName} template error");
        }
    }
}