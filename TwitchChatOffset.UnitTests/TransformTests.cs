using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset.UnitTests;

public class TransformTests
{
    private static long[] AllStartsEnds => [0, 1, 10, 100, 1000, -1, -10, -100, -1000, long.MinValue, long.MaxValue];
    private static long[] AllNegativeEnds => [-1, -10, -100, -1000, long.MinValue];
    private static Format[] AllFormats => [Format.Json, Format.JsonIndented, Format.Plaintext];

    private const string ContentOffsetSecondsTemplate = "\"content_offset_seconds\":0";
    private const string CommenterTemplate = "\"commenter\":{\"display_name\":\"JohnSmith\"}";
    private const string MessageTemplate = "\"message\":{\"body\":\"Hello, World!\"}";

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("\n")]
    [InlineData("\t")]
    [InlineData("  \t\n\r  ")]
    public void MTransform_EmptyOrWhitespaceJsonString_ThrowsJsonContentExceptionEmpty(string inputString)
    {
        long[] starts = AllStartsEnds;
        long[] ends = AllStartsEnds;
        Format[] formats = AllFormats;

        JsonContentException expectedException = JsonContentException.Empty();

        foreach (long start in starts)
        {
            foreach (long end in ends)
            {
                foreach (Format format in formats)
                {
                    void GetOutput() => Transform.MTransform(inputString, start, end, format);

                    JsonContentException exception = Assert.Throws<JsonContentException>(GetOutput);

                    Assert.Equal(expectedException.Message, exception.Message);
                }
            }
        }
    }

    [Theory]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("{(}")]
    [InlineData("()")]
    [InlineData("(")]
    public void MTransform_InvalidJsonString_ThrowsJsonException(string inputString)
    {
        long[] starts = AllStartsEnds;
        long[] ends = AllStartsEnds;
        Format[] formats = AllFormats;

        foreach (long start in starts)
        {
            foreach (long end in ends)
            {
                foreach (Format format in formats)
                {
                    void GetOutput() => Transform.MTransform(inputString, start, end, format);

                    Assert.ThrowsAny<JsonException>(GetOutput);
                }
            }
        }
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"comment\":{}}")]
    [InlineData("{\"comments\":[]}")]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{}}]}}")]
    public void ApplyOffset_Start0EndNegative_DoesNothing(string inputString)
    {
        JToken json = (JToken)JsonConvert.DeserializeObject(inputString)!;
        long start = 0;
        long[] ends = AllNegativeEnds;

        foreach (long end in ends)
        {
            Transform.ApplyOffset(json, start, end);
            string output = JsonConvert.SerializeObject(json);

            Assert.Equal(inputString, output);
        }
    }

    // all remaining tests assume that the conditions of the previous test aren't true

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"comment\":[]}")]
    [InlineData("{\"commentss\":[]}")]
    public void ApplyOffset_NoComments_ThrowsJsonContentExceptionNoComments(string inputString)
    {
        JToken json = (JToken)JsonConvert.DeserializeObject(inputString)!;
        long[] starts = AllStartsEnds;
        long[] ends = AllStartsEnds;

        JsonContentException expectedException = JsonContentException.NoComments();

        foreach (long start in starts)
        {
            foreach (long end in ends)
            {
                if (start == 0 && end < 0)
                    continue;

                void GetOutput() => Transform.ApplyOffset(json, start, end);

                JsonContentException exception = Assert.Throws<JsonContentException>(GetOutput);

                Assert.Equal(expectedException.Message, exception.Message);
            }
        }
    }
}