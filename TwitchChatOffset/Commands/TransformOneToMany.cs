using System.CommandLine;
using System.CommandLine.Binding;

namespace TwitchChatOffset.Commands;

public class TransformOneToMany : CommandBinder<TransformOneToMany.Data>
{
    public override Command PCommand { get; } = new("transform-one-to-many", "Transform a JSON Twitch file into many new files");

    private readonly Tokens tokens = new();

    protected override void AddTokens()
    {
        PCommand.Add(tokens.InputArgument);
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
            Arg(tokens.InputArgument),
            Arg(tokens.CsvArgument),
            Opt(tokens.OutputDirOption),
            Opt(tokens.FormatOption),
            Opt(tokens.QuietOption)
        );
    }

    public readonly record struct Data
    (
        string InputPath,
        string CsvPath,
        string OutputDir,
        Format PFormat,
        bool Quiet
    );

    protected override void Handle(Data data)
    {
        (string inputPath, string csvPath, string outputDir, Format format, bool quiet) = data;
        TwitchChatOffset.Transform.HandleTransformOneToMany(inputPath, csvPath, outputDir, format, quiet);
    }
}