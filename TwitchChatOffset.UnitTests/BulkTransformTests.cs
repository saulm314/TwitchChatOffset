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
}