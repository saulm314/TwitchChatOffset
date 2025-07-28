using TwitchChatOffset.CommandLine.Options;

namespace TwitchChatOffset.UnitTests;

public class BulkTransformTests
{
    [Theory]
    [InlineData((long)0, 0, OptionPriority.CSV)]
    [InlineData((long)1, 0, OptionPriority.CSV)]
    [InlineData((long)0, -1, OptionPriority.CSV)]
    [InlineData((long)0, long.MinValue, OptionPriority.CSV)]
    [InlineData(long.MaxValue, 0, OptionPriority.CSV)]
    [InlineData(long.MaxValue, long.MinValue, OptionPriority.CSV)]
    [InlineData((long)0, 1, OptionPriority.CLI)]
    [InlineData((long)-1, 0, OptionPriority.CLI)]
    [InlineData(long.MinValue, 0, OptionPriority.CLI)]
    [InlineData((long)0, long.MaxValue, OptionPriority.CLI)]
    [InlineData(long.MinValue, long.MaxValue, OptionPriority.CLI)]
    [InlineData(null, 0, OptionPriority.CSV)]
    [InlineData(null, -1, OptionPriority.CSV)]
    [InlineData(null, long.MinValue, OptionPriority.CSV)]
    [InlineData(null, 1, OptionPriority.CLI)]
    [InlineData(null, long.MaxValue, OptionPriority.CLI)]
    public void GetOptionPriority(long? csvOptionPriority, long cliOptionPriority, OptionPriority expectedOutput)
    {
        OptionPriority output = BulkTransform.GetOptionPriority(csvOptionPriority, cliOptionPriority);

        Assert.Equal(expectedOutput, output);
    }
}