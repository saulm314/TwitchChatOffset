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
    public void MTransformString_EmptyString_ThrowsJsonContentException(long start, long end, Format format)
    {
        string input = string.Empty;

        void GetOutput() => Transform.MTransform(input, start, end, format);

        JsonContentException e = Assert.Throws<JsonContentException>(GetOutput);
        Assert.Equal(JsonContentException.Empty, e.Message);
    }

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
    public void MTransformString_EmptyJToken_ThrowsJsonContentExceptionEmpty(long start, long end, Format format)
    {
        string input = "{}";

        void GetOutput() => Transform.MTransform(input, start, end, format);

        JsonContentException e = Assert.Throws<JsonContentException>(GetOutput);
        Assert.Equal(JsonContentException.NoComments, e.Message);
    }

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
    public void MTransformJToken_EmptyJToken_ThrowsJsonContentExceptionNoComments(long start, long end, Format format)
    {
        string inputString = "{}";
        JToken input = (JToken)JsonConvert.DeserializeObject(inputString)!;

        void GetOutput() => Transform.MTransform(input, start, end, format);

        JsonContentException e = Assert.Throws<JsonContentException>(GetOutput);
        Assert.Equal(JsonContentException.NoComments, e.Message);
    }
}