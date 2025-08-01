using TwitchChatOffset.CommandLine.Options;

namespace TwitchChatOffset.UnitTests;

public class MockNullableOption<T>(T value, bool valueSpecified) : INullableOption<T> where T : notnull
{
    public T Value => value;
    public bool ValueSpecified => valueSpecified;
}