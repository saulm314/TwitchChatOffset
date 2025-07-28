using TwitchChatOffset.CommandLine.Options;

namespace TwitchChatOffset.UnitTests;

public class MockNullableOption<TType>(TType value, bool valueSpecified) : INullableOption<TType> where TType : notnull
{
    public TType Value => value;
    public bool ValueSpecified => valueSpecified;
}