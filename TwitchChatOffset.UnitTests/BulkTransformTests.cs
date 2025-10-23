using TwitchChatOffset.CommandLine.Options;

namespace TwitchChatOffset.UnitTests;

public class BulkTransformTests
{
    [Theory]
    [InlineData(".", "transformedChat.json", @".\transformedChat.json")]
    [InlineData(@".\", "transformedChat.json", @".\transformedChat.json")]
    public void GetCombiedPathTest(string outputDir, string outputFile, string expectedOutput)
    {
        string output = BulkTransform.GetCombinedPath(outputDir, outputFile);

        Assert.Equal(expectedOutput, output);
    }

    [Theory]
    [InlineData("chat.json", "transformed", ".json", @"transformed\chat.json")]
    [InlineData("chat.json", "transformed", ".txt", @"transformed\chat.txt")]
    [InlineData("chat.json", "transformed", "", @"transformed\chat")]
    [InlineData("chat.json", @"transformed\", ".json", @"transformed\chat.json")]
    [InlineData("chat.json", @"transformed\", ".txt", @"transformed\chat.txt")]
    [InlineData("chat.json", @"transformed\", "", @"transformed\chat")]
    [InlineData("chat", "transformed", ".json", @"transformed\chat.json")]
    [InlineData("chat", "transformed", ".txt", @"transformed\chat.txt")]
    [InlineData("chat", "transformed", "", @"transformed\chat")]
    [InlineData("chat", @"transformed\", ".json", @"transformed\chat.json")]
    [InlineData("chat", @"transformed\", ".txt", @"transformed\chat.txt")]
    [InlineData("chat", @"transformed\", "", @"transformed\chat")]
    public void GetOutputPathTest(string inputFileName, string outputDir, string outputSuffix, string expectedOutput)
    {
        string output = BulkTransform.GetOutputPath(inputFileName, outputDir, outputSuffix);

        Assert.Equal(expectedOutput, output);
    }

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
    public void GetOptionPriorityTest(long? csvOptionPriority, long cliOptionPriority, OptionPriority expectedOutput)
    {
        OptionPriority output = BulkTransform.GetOptionPriority(csvOptionPriority, cliOptionPriority);

        Assert.Equal(expectedOutput, output);
    }

    [Theory]
    [InlineData((long)5, 2, true, 5)]
    [InlineData((long)2, 5, true, 2)]
    [InlineData(null, 2, true, 2)]
    [InlineData((long)5, 2, false, 5)]
    [InlineData((long)2, 5, false, 2)]
    [InlineData(null, 2, false, 2)]
    public void ResolveClashPrioritiseCsvStructTest(long? csvValue, long cliValue, bool cliValueImplicit, long expectedOutput)
    {
        ImplicitValue<long> cliOption = new(cliValue, cliValueImplicit);

        long output = BulkTransform.ResolveClashPrioritiseCsv(csvValue, cliOption);

        Assert.Equal(expectedOutput, output);
    }

    [Theory]
    [InlineData("5", "2", true, "5")]
    [InlineData("2", "5", true, "2")]
    [InlineData(null, "2", true, "2")]
    [InlineData("5", "2", false, "5")]
    [InlineData("2", "5", false, "2")]
    [InlineData(null, "2", false, "2")]
    public void ResolveClashPrioritiseCsvClassTest(string? csvValue, string cliValue, bool cliValueImplicit, string expectedOutput)
    {
        ImplicitValue<string> cliOption = new(cliValue, cliValueImplicit);

        string output = BulkTransform.ResolveClashPrioritiseCsv(csvValue, cliOption);

        Assert.Equal(expectedOutput, output);
    }

    [Theory]
    [InlineData((long)5, 2, true, 5)]
    [InlineData((long)2, 5, true, 2)]
    [InlineData(null, 2, true, 2)]
    [InlineData((long)5, 2, false, 2)]
    [InlineData((long)2, 5, false, 5)]
    [InlineData(null, 2, false, 2)]
    public void ResolveClashPrioritiseCliStructTest(long? csvValue, long cliValue, bool cliValueImplicit, long expectedOutput)
    {
        ImplicitValue<long> cliOption = new(cliValue, cliValueImplicit);

        long output = BulkTransform.ResolveClashPrioritiseCli(csvValue, cliOption);

        Assert.Equal(expectedOutput, output);
    }

    [Theory]
    [InlineData("5", "2", true, "5")]
    [InlineData("2", "5", true, "2")]
    [InlineData(null, "2", true, "2")]
    [InlineData("5", "2", false, "2")]
    [InlineData("2", "5", false, "5")]
    [InlineData(null, "2", false, "2")]
    public void ResolveClashPrioritiseCliClassTest(string? csvValue, string cliValue, bool cliValueImplicit, string expectedOutput)
    {
        ImplicitValue<string> cliOption = new(cliValue, cliValueImplicit);

        string output = BulkTransform.ResolveClashPrioritiseCli(csvValue, cliOption);

        Assert.Equal(expectedOutput, output);
    }
}