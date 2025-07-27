using TwitchChatOffset;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffsetUnitTests;

public class TransformTests
{
    private static long[] AllStartsEnds => [0, 1, 10, 100, 1000, -1, -10, -100, -1000, long.MinValue, long.MaxValue];
    private static long[] AllNegativeEnds => [-1, -10, -100, -1000, long.MinValue];
    private static Format[] AllFormats => [Format.Json, Format.JsonIndented, Format.Plaintext];

    private const string ContentOffsetSecondsTemplate = "\"content_offset_seconds\":0";
    private const string CommenterTemplate = "\"commenter\":{\"display_name\":\"JohnSmith\"}";
    private const string MessageTemplate = "\"message\":{\"body\":\"Hello, World!\"}";

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
        long[] ends = AllNegativeEnds;
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
    [InlineData($"{{\"comments\":[{{{CommenterTemplate},{MessageTemplate}}}]}}", 0)]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{{CommenterTemplate},{MessageTemplate}}}]}}", 1)]
    [InlineData($"{{\"comments\":[{{{CommenterTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}}]}}", 0)]
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

    [Theory]
    [InlineData($"{{\"comments\":[{{{CommenterTemplate},{MessageTemplate}}}]}}", 0)]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{{CommenterTemplate},{MessageTemplate}}}]}}", 1)]
    [InlineData($"{{\"comments\":[{{{CommenterTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}}]}}", 0)]
    public void MTransform_NoContentOffsetSecondsPlaintextFormat_ThrowsJsonContentExceptionNoContentOffsetSeconds(string inputString, int index)
    {
        long[] starts = AllStartsEnds;
        long[] ends = AllStartsEnds;
        Format format = Format.Plaintext;
        JToken input = (JToken)JsonConvert.DeserializeObject(inputString)!;

        JsonContentException expectedException = JsonContentException.NoContentOffsetSeconds(index);

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

    // we do these tests with no offset to ensure that the formatter has some comments left to parse
    [Theory]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{MessageTemplate}}}]}}", 0)]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{MessageTemplate}}}]}}", 1)]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}}]}}", 0)]
    public void MTransform_NoCommenterPlaintextFormat_ThrowsJsonContentExceptionNoCommenter(string inputString, int index)
    {
        long start = 0;
        long[] ends = AllNegativeEnds;
        Format format = Format.Plaintext;
        JToken input = (JToken)JsonConvert.DeserializeObject(inputString)!;

        JsonContentException expectedException = JsonContentException.NoCommenter(index);

        foreach (long end in ends)
        {
            void GetOutput1() => Transform.MTransform(inputString, start, end, format);
            void GetOutput2() => Transform.MTransform(inputString, start, end, format);

            JsonContentException exception1 = Assert.Throws<JsonContentException>(GetOutput1);
            JsonContentException exception2 = Assert.Throws<JsonContentException>(GetOutput2);

            Assert.Equal(expectedException.Message, exception1.Message);
            Assert.Equal(expectedException.Message, exception2.Message);
        }
    }

    [Theory]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},\"commenter\":{{}},{MessageTemplate}}}]}}", 0)]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},\"commenter\":{{}},{MessageTemplate}}}]}}", 1)]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},\"commenter\":{{}},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}}]}}", 0)]
    public void MTransform_NoDisplayNamePlaintextFormat_ThrowsJsonContentExceptionNoDisplayName(string inputString, int index)
    {
        long start = 0;
        long[] ends = AllNegativeEnds;
        Format format = Format.Plaintext;
        JToken input = (JToken)JsonConvert.DeserializeObject(inputString)!;

        JsonContentException expectedException = JsonContentException.NoDisplayName(index);

        foreach (long end in ends)
        {
            void GetOutput1() => Transform.MTransform(inputString, start, end, format);
            void GetOutput2() => Transform.MTransform(inputString, start, end, format);

            JsonContentException exception1 = Assert.Throws<JsonContentException>(GetOutput1);
            JsonContentException exception2 = Assert.Throws<JsonContentException>(GetOutput2);

            Assert.Equal(expectedException.Message, exception1.Message);
            Assert.Equal(expectedException.Message, exception2.Message);
        }
    }

    [Theory]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate}}}]}}", 0)]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate}}}]}}", 1)]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}}]}}", 0)]
    public void MTransform_NoMessagePlaintextFormat_ThrowsJsonContentExceptionNoMessage(string inputString, int index)
    {
        long start = 0;
        long[] ends = AllNegativeEnds;
        Format format = Format.Plaintext;
        JToken input = (JToken)JsonConvert.DeserializeObject(inputString)!;

        JsonContentException expectedException = JsonContentException.NoMessage(index);

        foreach (long end in ends)
        {
            void GetOutput1() => Transform.MTransform(inputString, start, end, format);
            void GetOutput2() => Transform.MTransform(inputString, start, end, format);

            JsonContentException exception1 = Assert.Throws<JsonContentException>(GetOutput1);
            JsonContentException exception2 = Assert.Throws<JsonContentException>(GetOutput2);

            Assert.Equal(expectedException.Message, exception1.Message);
            Assert.Equal(expectedException.Message, exception2.Message);
        }
    }

    [Theory]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},\"message\":{{}}}}]}}", 0)]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},\"message\":{{}}}}]}}", 1)]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},\"message\":{{}}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}}]}}", 0)]
    public void MTransform_NoBodyPlaintextFormat_ThrowsJsonContentExceptionNoBody(string inputString, int index)
    {
        long start = 0;
        long[] ends = AllNegativeEnds;
        Format format = Format.Plaintext;
        JToken input = (JToken)JsonConvert.DeserializeObject(inputString)!;

        JsonContentException expectedException = JsonContentException.NoBody(index);

        foreach (long end in ends)
        {
            void GetOutput1() => Transform.MTransform(inputString, start, end, format);
            void GetOutput2() => Transform.MTransform(inputString, start, end, format);

            JsonContentException exception1 = Assert.Throws<JsonContentException>(GetOutput1);
            JsonContentException exception2 = Assert.Throws<JsonContentException>(GetOutput2);

            Assert.Equal(expectedException.Message, exception1.Message);
            Assert.Equal(expectedException.Message, exception2.Message);
        }
    }
}