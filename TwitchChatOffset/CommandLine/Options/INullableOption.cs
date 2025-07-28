namespace TwitchChatOffset.CommandLine.Options;

public interface INullableOption<TType> where TType : notnull
{
    TType Value { get; }
    bool ValueSpecified { get; }
}