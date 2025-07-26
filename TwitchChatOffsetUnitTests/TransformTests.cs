using TwitchChatOffset;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffsetUnitTests;

public class TransformTests
{
    [Theory]
    [InlineData(0, 0, Format.Json)]
    [InlineData(0, 0, Format.JsonIndented)]
    [InlineData(0, 0, Format.Plaintext)]
    [InlineData(0, long.MinValue, Format.Json)]
    [InlineData(0, long.MinValue, Format.JsonIndented)]
    [InlineData(0, long.MinValue, Format.Plaintext)]
    [InlineData(0, long.MaxValue, Format.Json)]
    [InlineData(0, long.MaxValue, Format.JsonIndented)]
    [InlineData(0, long.MaxValue, Format.Plaintext)]
    [InlineData(long.MinValue, 0, Format.Json)]
    [InlineData(long.MinValue, 0, Format.JsonIndented)]
    [InlineData(long.MinValue, 0, Format.Plaintext)]
    [InlineData(long.MinValue, long.MinValue, Format.Json)]
    [InlineData(long.MinValue, long.MinValue, Format.JsonIndented)]
    [InlineData(long.MinValue, long.MinValue, Format.Plaintext)]
    [InlineData(long.MinValue, long.MaxValue, Format.Json)]
    [InlineData(long.MinValue, long.MaxValue, Format.JsonIndented)]
    [InlineData(long.MinValue, long.MaxValue, Format.Plaintext)]
    [InlineData(long.MaxValue, 0, Format.Json)]
    [InlineData(long.MaxValue, 0, Format.JsonIndented)]
    [InlineData(long.MaxValue, 0, Format.Plaintext)]
    [InlineData(long.MaxValue, long.MinValue, Format.Json)]
    [InlineData(long.MaxValue, long.MinValue, Format.JsonIndented)]
    [InlineData(long.MaxValue, long.MinValue, Format.Plaintext)]
    [InlineData(long.MaxValue, long.MaxValue, Format.Json)]
    [InlineData(long.MaxValue, long.MaxValue, Format.JsonIndented)]
    [InlineData(long.MaxValue, long.MaxValue, Format.Plaintext)]
    public void MTransformString_EmptyString_ThrowsJsonContentExceptionEmpty(long start, long end, Format format)
    {
        string input = string.Empty;

        void GetOutput() => Transform.MTransform(input, start, end, format);

        JsonContentException e = Assert.Throws<JsonContentException>(GetOutput);
        Assert.Equal(JsonContentException.Empty, e.Message);
    }

    [Theory]
    [InlineData("{}", 0, 0, Format.Json)]
    [InlineData("{}", 0, 0, Format.JsonIndented)]
    [InlineData("{}", 0, 0, Format.Plaintext)]
    [InlineData("{}", 0, long.MinValue, Format.Json)]
    [InlineData("{}", 0, long.MinValue, Format.JsonIndented)]
    [InlineData("{}", 0, long.MinValue, Format.Plaintext)]
    [InlineData("{}", 0, long.MaxValue, Format.Json)]
    [InlineData("{}", 0, long.MaxValue, Format.JsonIndented)]
    [InlineData("{}", 0, long.MaxValue, Format.Plaintext)]
    [InlineData("{}", long.MinValue, 0, Format.Json)]
    [InlineData("{}", long.MinValue, 0, Format.JsonIndented)]
    [InlineData("{}", long.MinValue, 0, Format.Plaintext)]
    [InlineData("{}", long.MinValue, long.MinValue, Format.Json)]
    [InlineData("{}", long.MinValue, long.MinValue, Format.JsonIndented)]
    [InlineData("{}", long.MinValue, long.MinValue, Format.Plaintext)]
    [InlineData("{}", long.MinValue, long.MaxValue, Format.Json)]
    [InlineData("{}", long.MinValue, long.MaxValue, Format.JsonIndented)]
    [InlineData("{}", long.MinValue, long.MaxValue, Format.Plaintext)]
    [InlineData("{}", long.MaxValue, 0, Format.Json)]
    [InlineData("{}", long.MaxValue, 0, Format.JsonIndented)]
    [InlineData("{}", long.MaxValue, 0, Format.Plaintext)]
    [InlineData("{}", long.MaxValue, long.MinValue, Format.Json)]
    [InlineData("{}", long.MaxValue, long.MinValue, Format.JsonIndented)]
    [InlineData("{}", long.MaxValue, long.MinValue, Format.Plaintext)]
    [InlineData("{}", long.MaxValue, long.MaxValue, Format.Json)]
    [InlineData("{}", long.MaxValue, long.MaxValue, Format.JsonIndented)]
    [InlineData("{}", long.MaxValue, long.MaxValue, Format.Plaintext)]
    public void MTransformString_NoComments_ThrowsJsonContentExceptionNoComments(string input, long start, long end, Format format)
    {
        void GetOutput() => Transform.MTransform(input, start, end, format);

        JsonContentException e = Assert.Throws<JsonContentException>(GetOutput);
        Assert.Equal(JsonContentException.NoComments, e.Message);
    }

    [Theory]
    [InlineData("{}", 0, 0, Format.Json)]
    [InlineData("{}", 0, 0, Format.JsonIndented)]
    [InlineData("{}", 0, 0, Format.Plaintext)]
    [InlineData("{}", 0, long.MinValue, Format.Json)]
    [InlineData("{}", 0, long.MinValue, Format.JsonIndented)]
    [InlineData("{}", 0, long.MinValue, Format.Plaintext)]
    [InlineData("{}", 0, long.MaxValue, Format.Json)]
    [InlineData("{}", 0, long.MaxValue, Format.JsonIndented)]
    [InlineData("{}", 0, long.MaxValue, Format.Plaintext)]
    [InlineData("{}", long.MinValue, 0, Format.Json)]
    [InlineData("{}", long.MinValue, 0, Format.JsonIndented)]
    [InlineData("{}", long.MinValue, 0, Format.Plaintext)]
    [InlineData("{}", long.MinValue, long.MinValue, Format.Json)]
    [InlineData("{}", long.MinValue, long.MinValue, Format.JsonIndented)]
    [InlineData("{}", long.MinValue, long.MinValue, Format.Plaintext)]
    [InlineData("{}", long.MinValue, long.MaxValue, Format.Json)]
    [InlineData("{}", long.MinValue, long.MaxValue, Format.JsonIndented)]
    [InlineData("{}", long.MinValue, long.MaxValue, Format.Plaintext)]
    [InlineData("{}", long.MaxValue, 0, Format.Json)]
    [InlineData("{}", long.MaxValue, 0, Format.JsonIndented)]
    [InlineData("{}", long.MaxValue, 0, Format.Plaintext)]
    [InlineData("{}", long.MaxValue, long.MinValue, Format.Json)]
    [InlineData("{}", long.MaxValue, long.MinValue, Format.JsonIndented)]
    [InlineData("{}", long.MaxValue, long.MinValue, Format.Plaintext)]
    [InlineData("{}", long.MaxValue, long.MaxValue, Format.Json)]
    [InlineData("{}", long.MaxValue, long.MaxValue, Format.JsonIndented)]
    [InlineData("{}", long.MaxValue, long.MaxValue, Format.Plaintext)]
    public void MTransformJToken_NoComments_ThrowsJsonContentExceptionNoComments(string inputString, long start, long end, Format format)
    {
        JToken input = (JToken)JsonConvert.DeserializeObject(inputString)!;

        void GetOutput() => Transform.MTransform(input, start, end, format);

        JsonContentException e = Assert.Throws<JsonContentException>(GetOutput);
        Assert.Equal(JsonContentException.NoComments, e.Message);
    }

    [Theory]
    [InlineData("{\"comments\":[{}]}", 0, 0, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", 0, 0, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", 0, 0, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", 0, long.MinValue, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", 0, long.MinValue, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", 0, long.MinValue, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", 0, long.MaxValue, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", 0, long.MaxValue, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", 0, long.MaxValue, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, 0, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, 0, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, 0, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, long.MinValue, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, long.MinValue, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, long.MinValue, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, long.MaxValue, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, long.MaxValue, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, long.MaxValue, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, 0, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, 0, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, 0, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, long.MinValue, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, long.MinValue, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, long.MinValue, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, long.MaxValue, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, long.MaxValue, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, long.MaxValue, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, 0, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, 0, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, 0, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, long.MinValue, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, long.MinValue, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, long.MinValue, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, long.MaxValue, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, long.MaxValue, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, long.MaxValue, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, 0, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, 0, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, 0, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, long.MinValue, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, long.MinValue, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, long.MinValue, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, long.MaxValue, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, long.MaxValue, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, long.MaxValue, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, 0, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, 0, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, 0, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, long.MinValue, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, long.MinValue, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, long.MinValue, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, long.MaxValue, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, long.MaxValue, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, long.MaxValue, Format.Plaintext, 1)]
    public void MTransformString_NoContentOffsetSeconds_ThrowsJsonContentExceptionNoContentOffsetSeconds(string input, long start, long end, Format format,
        int i)
    {
        void GetOutput() => Transform.MTransform(input, start, end, format);

        JsonContentException e = Assert.Throws<JsonContentException>(GetOutput);
        Assert.Equal(JsonContentException.NoContentOffsetSeconds(i), e.Message);
    }

    [Theory]
    [InlineData("{\"comments\":[{}]}", 0, 0, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", 0, 0, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", 0, 0, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", 0, long.MinValue, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", 0, long.MinValue, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", 0, long.MinValue, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", 0, long.MaxValue, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", 0, long.MaxValue, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", 0, long.MaxValue, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, 0, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, 0, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, 0, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, long.MinValue, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, long.MinValue, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, long.MinValue, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, long.MaxValue, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, long.MaxValue, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", long.MinValue, long.MaxValue, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, 0, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, 0, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, 0, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, long.MinValue, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, long.MinValue, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, long.MinValue, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, long.MaxValue, Format.Json, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, long.MaxValue, Format.JsonIndented, 0)]
    [InlineData("{\"comments\":[{}]}", long.MaxValue, long.MaxValue, Format.Plaintext, 0)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, 0, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, 0, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, 0, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, long.MinValue, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, long.MinValue, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, long.MinValue, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, long.MaxValue, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, long.MaxValue, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", 0, long.MaxValue, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, 0, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, 0, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, 0, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, long.MinValue, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, long.MinValue, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, long.MinValue, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, long.MaxValue, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, long.MaxValue, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MinValue, long.MaxValue, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, 0, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, 0, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, 0, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, long.MinValue, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, long.MinValue, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, long.MinValue, Format.Plaintext, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, long.MaxValue, Format.Json, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, long.MaxValue, Format.JsonIndented, 1)]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":0},{}]}", long.MaxValue, long.MaxValue, Format.Plaintext, 1)]
    public void MTransformJToken_NoContentOffsetSeconds_ThrowsJsonContentExceptionNoContentOffsetSeconds(string inputString, long start, long end, Format format,
        int i)
    {
        JToken input = (JToken)JsonConvert.DeserializeObject(inputString)!;

        void GetOutput() => Transform.MTransform(input, start, end, format);

        JsonContentException e = Assert.Throws<JsonContentException>(GetOutput);
        Assert.Equal(JsonContentException.NoContentOffsetSeconds(i), e.Message);
    }
}