using System.CommandLine;
using System.CommandLine.Binding;

namespace TwitchChatOffset.Commands;

public class TransformManyToMany : CommandBinder<TransformManyToMany.Data>
{
    public override Command PCommand { get; } = new("transform-many-to-many", "Transform many Json Twitch files and generate new files for each transformation");

    private readonly Tokens tokens = new();

    protected override void AddTokens()
    {
        PCommand.Add(tokens.CsvArgument);
        PCommand.Add(tokens.OutputDirOption);
        PCommand.Add(tokens.FormatOption);
        PCommand.Add(tokens.QuietOption);
    }

    protected override Data GetBoundValue(BindingContext bindingContext)
    {
        T Arg<T>(Argument<T> argument) => GetArgValue(argument, bindingContext);
        T Opt<T>(Option<T> option) => GetOptValue(option, bindingContext);
        return new
        (
            Arg(tokens.CsvArgument),
            Opt(tokens.OutputDirOption),
            Opt(tokens.FormatOption),
            Opt(tokens.QuietOption)
        );
    }

    public readonly record struct Data
    (
        string CsvPath,
        string OutputDir,
        Format PFormat,
        bool Quiet
    );

    protected override void Handle(Data data)
    {
        (string csvPath, string outputDir, Format format, bool quiet) = data;
        TransformHandler.HandleTransformManyToMany(csvPath, outputDir, format, quiet);
    }
}