using TwitchChatOffset;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffsetUnitTests;

public class TransformTests
{
    [Fact]
    public void MTransform_EmptyString_ThrowsJsonContentExceptionEmpty()
    {
        long[] starts = [0, 1, 10, 100, 1000, -1, -10, -100, -1000, long.MinValue, long.MaxValue];
        long[] ends = [0, 1, 10, 100, 1000, -1, -10, -100, -1000, long.MinValue, long.MaxValue];
        Format[] formats = [Format.Json, Format.JsonIndented, Format.Plaintext];
        string inputString = string.Empty;

        foreach (long start in starts)
        {
            foreach (long end in ends)
            {
                foreach (Format format in formats)
                {
                    void GetOutput() => Transform.MTransform(inputString, start, end, format);

                    JsonContentException e = Assert.Throws<JsonContentException>(GetOutput);
                    Assert.Equal(JsonContentException.Empty, e.Message);
                }
            }
        }
    }

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
}