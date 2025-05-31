using System.CommandLine;
using System.CommandLine.Binding;
using static TwitchChatOffset.Tokens;

namespace TwitchChatOffset.Commands;

public class TransformAll : CommandBinder<TransformAll.Data>
{
    public override Command PCommand { get; } = new("transform-all", "Transform all files in a directory whose name matches a search pattern");

    protected override Data GetBoundValue(BindingContext bindingContext)
    {
        T Arg<T>(Argument<T> argument) => GetArgValue(argument, bindingContext);
        T Opt<T>(Option<T> option) => GetOptValue(option, bindingContext);
        return new
        (
            Arg(SuffixArgument),
            Opt(InputDirOption),
            Opt(SearchPatternOption),
            Opt(OutputDirOption),
            Opt(FormatOption),
            Opt(QuietOption),
            Opt(StartOption),
            Opt(EndOption)
        );
    }

    public readonly record struct Data
    (
        string Suffix,
        string InputDir,
        string SearchPattern,
        string OutputDir,
        Format PFormat,
        bool Quiet,
        long Start,
        long End
    );

    protected override void Handle(Data data)
    {
        (string suffix, string inputDir, string searchPattern, string outputDir, Format format, bool quiet, long start, long end) = data;
        TransformHandler.HandleTransformAll(suffix, inputDir, searchPattern, outputDir, format, quiet, start, end);
    }
}