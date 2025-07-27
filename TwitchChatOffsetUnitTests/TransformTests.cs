using TwitchChatOffset;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffsetUnitTests;

public class TransformTests
{
    private static long[] AllStartsEnds => [0, 1, 10, 100, 1000, -1, -10, -100, -1000, long.MinValue, long.MaxValue];
    private static Format[] AllFormats => [Format.Json, Format.JsonIndented, Format.Plaintext];

    [Fact]
    public void MTransform_EmptyString_ThrowsJsonContentExceptionEmpty()
    {
        long[] starts = AllStartsEnds;
        long[] ends = AllStartsEnds;
        Format[] formats = AllFormats;
        string inputString = string.Empty;

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

    // subsequent tests should take care to avoid testing for cases that this test covers already
    [Theory]
    [InlineData("{}", Format.Json, "{}")]
    [InlineData("{}", Format.JsonIndented, "{}")]
    [InlineData("{\"comment\":[]}", Format.Json, "{\"comment\":[]}")]
    [InlineData("{\"comment\":[]}", Format.JsonIndented, "{\r\n  \"comment\": []\r\n}")]
    [InlineData("{\"comments\":[]}", Format.Json, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[]}", Format.JsonIndented, "{\r\n  \"comments\": []\r\n}")]
    public void MTransform_NoOffsetJsonFormat_ReturnsFormattedJson(string inputString, Format format, string expectedOutput)
    {
        long start = 0;
        long[] ends = [-1, -10, -100, -1000, long.MinValue];
        JToken input = (JToken)JsonConvert.DeserializeObject(inputString)!;

        foreach (long end in ends)
        {
            string output1 = Transform.MTransform(inputString, start, end, format);
            string output2 = Transform.MTransform(input, start, end, format);

            Assert.Equal(expectedOutput, output1);
            Assert.Equal(expectedOutput, output2);
        }
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"comment\":[]}")]
    [InlineData("{\"commentss\":[]}")]
    public void MTransform_NoCommentsWithOffset_ThrowsJsonContentExceptionNoComments(string inputString)
    {
        long[] starts = AllStartsEnds;
        long[] ends = AllStartsEnds;
        Format[] formats = AllFormats;
        JToken input = (JToken)JsonConvert.DeserializeObject(inputString)!;

        JsonContentException expectedException = JsonContentException.NoComments();

        foreach (long start in starts)
        {
            foreach (long end in ends)
            {
                if (start == 0 && end < 0)
                    continue;
                foreach (Format format in formats)
                {
                    void GetOutput1() => Transform.MTransform(inputString, start, end, format);
                    void GetOutput2() => Transform.MTransform(input, start, end, format);

                    JsonContentException exception1 = Assert.Throws<JsonContentException>(GetOutput1);
                    JsonContentException exception2 = Assert.Throws<JsonContentException>(GetOutput2);

                    Assert.Equal(expectedException.Message, exception1.Message);
                    Assert.Equal(expectedException.Message, exception2.Message);
                }
            }
        }
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"comment\":[]}")]
    [InlineData("{\"commentss\":[]}")]
    public void MTransform_NoCommentsPlaintextFormat_ThrowsJsonContentExceptionNoComments(string inputString)
    {
        long[] starts = AllStartsEnds;
        long[] ends = AllStartsEnds;
        Format format = Format.Plaintext;
        JToken input = (JToken)JsonConvert.DeserializeObject(inputString)!;

        JsonContentException expectedException = JsonContentException.NoComments();

        foreach (long start in starts)
        {
            foreach (long end in ends)
            {
                void GetOutput1() => Transform.MTransform(inputString, start, end, format);
                void GetOutput2() => Transform.MTransform(input, start, end, format);

                JsonContentException exception1 = Assert.Throws<JsonContentException>(GetOutput1);
                JsonContentException exception2 = Assert.Throws<JsonContentException>(GetOutput2);

                Assert.Equal(expectedException.Message, exception1.Message);
                Assert.Equal(expectedException.Message, exception2.Message);
            }
        }
    }

    [Theory]
    [InlineData("{\"comments\":[{}]}", 0)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 1)]
    [InlineData("{\"comments\":[{},{\"content_offset_seconds\":0}]}", 0)]
    public void MTransform_NoContentOffsetSecondsWithOffset_ThrowsJsonContentExceptionNoContentOffsetSeconds(string inputString, int index)
    {
        long[] starts = AllStartsEnds;
        long[] ends = AllStartsEnds;
        Format[] formats = AllFormats;
        JToken input = (JToken)JsonConvert.DeserializeObject(inputString)!;

        JsonContentException expectedException = JsonContentException.NoContentOffsetSeconds(index);

        foreach (long start in starts)
        {
            foreach (long end in ends)
            {
                if (start == 0 && end < 0)
                    continue;
                foreach (Format format in formats)
                {
                    void GetOutput1() => Transform.MTransform(inputString, start, end, format);
                    void GetOutput2() => Transform.MTransform(input, start, end, format);

                    JsonContentException exception1 = Assert.Throws<JsonContentException>(GetOutput1);
                    JsonContentException exception2 = Assert.Throws<JsonContentException>(GetOutput2);

                    Assert.Equal(expectedException.Message, exception1.Message);
                    Assert.Equal(expectedException.Message, exception2.Message);
                }
            }
        }
    }
}