namespace TwitchChatOffset.Options.Optimisations;

// for each optimisation, it is assumed that all optimisations above it also apply
public enum TransformManyOptimisation
{
    None,
    SameInputFile,
    SameOffset,
    SameFormat,
    SameSubtitleOptions,
    Same
}