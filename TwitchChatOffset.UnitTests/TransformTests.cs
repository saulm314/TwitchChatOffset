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
            Transform.ApplyOffset(json.DeepClone(), start, end);
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

                void GetOutput() => Transform.ApplyOffset(json.DeepClone(), start, end);

                JsonContentException exception = Assert.Throws<JsonContentException>(GetOutput);

                Assert.Equal(expectedException.Message, exception.Message);
            }
        }
    }

    [Theory]
    [InlineData($"{{\"comments\":[{{{CommenterTemplate},{MessageTemplate}}}]}}", 0)]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{{CommenterTemplate},{MessageTemplate}}}]}}", 1)]
    [InlineData($"{{\"comments\":[{{{CommenterTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}}]}}", 0)]
    public void ApplyOffset_NoContentOffsetSeconds_ThrowsJsonContentExceptionNoContentOffsetSeconds(string inputString, int index)
    {
        JToken json = (JToken)JsonConvert.DeserializeObject(inputString)!;
        long[] starts = AllStartsEnds;
        long[] ends = AllStartsEnds;

        JsonContentException expectedException = JsonContentException.NoContentOffsetSeconds(index);

        foreach (long start in starts)
        {
            foreach (long end in ends)
            {
                if (start == 0 && end < 0)
                    continue;

                void GetOutput() => Transform.ApplyOffset(json.DeepClone(), start, end);

                JsonContentException exception = Assert.Throws<JsonContentException>(GetOutput);

                Assert.Equal(expectedException.Message, exception.Message);
            }
        }
    }

    [Theory]
    [InlineData("{\"comments\":[]}", 0, -1, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[]}", 0, long.MinValue, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[]}", long.MaxValue, 0, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 0, -1, "{\"comments\":[{\"content_offset_seconds\":5}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 0, -1, "{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 0, 4, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 0, 5, "{\"comments\":[{\"content_offset_seconds\":5}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 3, -1, "{\"comments\":[{\"content_offset_seconds\":2}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 3, 5, "{\"comments\":[{\"content_offset_seconds\":2}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 6, -1, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 6, 7, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 5, 5, "{\"comments\":[{\"content_offset_seconds\":0}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 5, 4, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 4, 4, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 6, 6, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 5, -1, "{\"comments\":[{\"content_offset_seconds\":0}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 3, 15, "{\"comments\":[{\"content_offset_seconds\":2}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 3, 17, "{\"comments\":[{\"content_offset_seconds\":2},{\"content_offset_seconds\":14}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 3, -1, "{\"comments\":[{\"content_offset_seconds\":2},{\"content_offset_seconds\":14}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 5, -1, "{\"comments\":[{\"content_offset_seconds\":0},{\"content_offset_seconds\":12}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 5, 16, "{\"comments\":[{\"content_offset_seconds\":0}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 5, 17, "{\"comments\":[{\"content_offset_seconds\":0},{\"content_offset_seconds\":12}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 6, 17, "{\"comments\":[{\"content_offset_seconds\":11}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 6, -1, "{\"comments\":[{\"content_offset_seconds\":11}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 6, 16, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 6, 18, "{\"comments\":[{\"content_offset_seconds\":11}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 17, 17, "{\"comments\":[{\"content_offset_seconds\":0}]}")]
    public void ApplyOffset_ValidInput_AppliesOffset(string inputString, long start, long end, string expectedOutput)
    {
        JToken json = (JToken)JsonConvert.DeserializeObject(inputString)!;

        Transform.ApplyOffset(json, start, end);
        string output = JsonConvert.SerializeObject(json);

        Assert.Equal(expectedOutput, output);
    }
}