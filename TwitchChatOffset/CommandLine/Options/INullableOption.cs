namespace TwitchChatOffset.CommandLine.Options;

public interface INullableOption<T> where T : notnull
{
    T Value { get; }
    bool ValueSpecified { get; }
}