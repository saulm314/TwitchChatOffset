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
        PCommand.Add(tokens.StartOption);
        PCommand.Add(tokens.EndOption);
        PCommand.Add(tokens.FormatOption);
        PCommand.Add(tokens.OutputDirOption);
        PCommand.Add(tokens.QuietOption);
    }

    protected override Data GetBoundValue(BindingContext bindingContext)
    {
        T Arg<T>(Argument<T> argument) => GetArgValue(argument, bindingContext);
        T Opt<T>(Option<T> option) => GetOptValue(option, bindingContext);
        return new
        (
            Arg(tokens.CsvArgument),
            Opt(tokens.StartOption),
            Opt(tokens.EndOption),
            Opt(tokens.FormatOption),
            Opt(tokens.OutputDirOption),
            Opt(tokens.QuietOption)
        );
    }

    public readonly record struct Data
    (
        string CsvPath,
        long Start,
        long End,
        Format PFormat,
        string OutputDir,
        bool Quiet
    );

    protected override void Handle(Data data)
    {
        (string csvPath, long start, long end, Format format, string outputDir, bool quiet) = data;
        TwitchChatOffset.Transform.HandleTransformManyToMany(csvPath, start, end, format, outputDir, quiet);
    }
}