namespace TwitchChatOffset.Options.Optimisations;

// the optimisations get more intense with each value
public enum Optimisation
{
    None,
    SameInputFile,
    SameOffset,
    SameFormatSameSubtitleOptions, // if the format is not a subtitle format then subtitle options can be different
    Same
}