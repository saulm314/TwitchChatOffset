using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset.UnitTests;

public class TransformTests
{
    private static long[] AllNegativeEnds => [-1, -10, -100, -1000, long.MinValue];

    private const string ContentOffsetSecondsTemplate = "\"content_offset_seconds\":0";
    private const string CommenterTemplate = "\"commenter\":{\"display_name\":\"JohnSmith\"}";
    private const string MessageTemplate = "\"message\":{\"body\":\"Hello, World!\"}";

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
}