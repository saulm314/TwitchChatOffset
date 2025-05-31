using System.CommandLine;
using System.CommandLine.Binding;
using static TwitchChatOffset.Tokens;

namespace TwitchChatOffset.Commands;

public class TransformOneToMany : CommandBinder<TransformOneToMany.Data>
{
    public override Command PCommand { get; } = new("transform-one-to-many", "Transform a JSON Twitch file into many new files");

    protected override Data GetBoundValue(BindingContext bindingContext)
    {
        T Arg<T>(Argument<T> argument) => GetArgValue(argument, bindingContext);
        T Opt<T>(Option<T> option) => GetOptValue(option, bindingContext);
        return new
        (
            Arg(InputArgument),
            Arg(CsvArgument),
            Opt(OutputDirOption),
            Opt(FormatOption),
            Opt(QuietOption)
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
        TransformHandler.HandleTransformOneToMany(inputPath, csvPath, outputDir, format, quiet);
    }
}