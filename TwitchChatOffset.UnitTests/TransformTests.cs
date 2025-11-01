using TwitchChatOffset.Options.Groups;
using TwitchChatOffset.Json;
using TwitchChatOffset.Ytt;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YTSubConverter.Shared;

namespace TwitchChatOffset.UnitTests;

public class TransformTests
{
    private static long[] AllStartsEnds => [0, 1, 10, 100, 1000, -1, -10, -100, -1000, long.MinValue, long.MaxValue];
    private static long[] AllNegativeEnds => [-1, -10, -100, -1000, long.MinValue];
    private static Format[] AllFormats => [Format.Json, Format.JsonIndented, Format.Ytt, Format.Plaintext];
    private static SubtitleOptions DefaultSubtitleOptions => new()
    {
        Position = new(AnchorPoint.TopLeft, true),
        MaxMessages = new(4, true),
        MaxCharsPerLine = new(40, true),
        WindowOpacity = new(0, true),
        TextColor = new("white", true),
        SectionOptions = new()
        {
            Scale = new(0.0, true),
            Shadow = new(Shadow.Glow, true),
            BackgroundOpacity = new(0, true),
            ShadowColor = new("black", true),
            BackgroundColor = new("black", true)
        }
    };

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
    public void DoTransform_EmptyOrWhitespaceJsonString_ThrowsJsonContentExceptionEmpty(string inputString)
    {
        long[] starts = AllStartsEnds;
        long[] ends = AllStartsEnds;
        Format[] formats = AllFormats;

        JsonContentException.Empty expectedException = new();

        foreach (long start in starts)
        {
            foreach (long end in ends)
            {
                foreach (Format format in formats)
                {
                    TransformOptions options = new()
                    {
                        Start = new(start, true),
                        End = new(end, true),
                        Format = new(format, true),
                        SubtitleOptions = DefaultSubtitleOptions
                    };
                    void DoTransform() => Transform.DoTransform(inputString, options);

                    JsonContentException.Empty exception = Assert.Throws<JsonContentException.Empty>(DoTransform);
                    Assert.Equal(expectedException.Message, exception.Message);

                    #pragma warning disable CS0162
                    continue; Transform.DoTransform(inputString, options);
                    #pragma warning restore CS0162
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
    public void DoTransform_InvalidJsonString_ThrowsJsonException(string inputString)
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
                    TransformOptions options = new()
                    {
                        Start = new(start, true),
                        End = new(end, true),
                        Format = new(format, true),
                        SubtitleOptions = DefaultSubtitleOptions
                    };
                    void DoTransform() => Transform.DoTransform(inputString, options);

                    Assert.ThrowsAny<JsonException>(DoTransform);

                    #pragma warning disable CS0162
                    continue; Transform.DoTransform(inputString, options);
                    #pragma warning restore CS0162
                }
            }
        }
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"comment\":{}}")]
    [InlineData("{\"comments\":[]}")]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{}}]}}")]
    public void ApplyOffset_Start0EndNegativeDelay0_DoesNothing(string inputString)
    {
        JToken json = JsonUtils.Deserialize(inputString);
        long start = 0;
        long[] ends = AllNegativeEnds;
        long delay = 0;

        foreach (long end in ends)
        {
            TransformOptions options = new()
            {
                Start = new(start, true),
                End = new(end, true),
                Delay = new(delay, true),
                SubtitleOptions = DefaultSubtitleOptions
            };
            Transform.ApplyOffset(json.DeepClone(), options);
            string output = JsonConvert.SerializeObject(json);

            Assert.Equal(inputString, output);
        }
    }

    // all remaining ApplyOffset tests assume that the conditions of the previous test aren't true

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"comment\":[]}")]
    [InlineData("{\"commentss\":[]}")]
    public void ApplyOffset_NoComments_ThrowsJsonContentExceptionPropertyNotFound(string inputString)
    {
        JToken json = JsonUtils.Deserialize(inputString);
        long[] starts = AllStartsEnds;
        long[] ends = AllStartsEnds;
        long delay = 0;

        JsonContentException.PropertyNotFound expectedException = new(string.Empty, "comments");

        foreach (long start in starts)
        {
            foreach (long end in ends)
            {
                if (start == 0 && end < 0 && delay <= 0)
                    continue;

                TransformOptions options = new()
                {
                    Start = new(start, true),
                    End = new(end, true),
                    SubtitleOptions = DefaultSubtitleOptions
                };
                void ApplyOffset() => Transform.ApplyOffset(json.DeepClone(), options);

                JsonContentException.PropertyNotFound exception = Assert.Throws<JsonContentException.PropertyNotFound>(ApplyOffset);
                Assert.Equal(expectedException.Message, exception.Message);

                #pragma warning disable CS0162
                continue; Transform.ApplyOffset(json.DeepClone(), options);
                #pragma warning restore CS0162
            }
        }
    }

    [Theory]
    [InlineData($"{{\"comments\":[{{{CommenterTemplate},{MessageTemplate}}}]}}")]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{{CommenterTemplate},{MessageTemplate}}}]}}")]
    [InlineData($"{{\"comments\":[{{{CommenterTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}}]}}")]
    public void ApplyOffset_NoContentOffsetSeconds_ThrowsJsonContentExceptionPropertyNotFound(string inputString)
    {
        JToken json = JsonUtils.Deserialize(inputString);
        long[] starts = AllStartsEnds;
        long[] ends = AllStartsEnds;
        long delay = 0;

        string expectedPathRegex = @"comments\[\d*\]";
        string expectedPropertyName = "content_offset_seconds";

        foreach (long start in starts)
        {
            foreach (long end in ends)
            {
                if (start == 0 && end < 0 && delay <= 0)
                    continue;

                TransformOptions options = new()
                {
                    Start = new(start, true),
                    End = new(end, true),
                    SubtitleOptions = DefaultSubtitleOptions
                };
                void ApplyOffset() => Transform.ApplyOffset(json.DeepClone(), options);

                JsonContentException.PropertyNotFound exception = Assert.Throws<JsonContentException.PropertyNotFound>(ApplyOffset);
                Assert.Matches(expectedPathRegex, exception.Path);
                Assert.Equal(expectedPropertyName, exception.PropertyName);

                #pragma warning disable CS0162
                continue; Transform.ApplyOffset(json.DeepClone(), options);
                #pragma warning restore CS0162
            }
        }
    }

    [Theory]
    [InlineData("{\"comments\":[]}", 0, -1, 0, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[]}", 0, long.MinValue, 0, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[]}", long.MaxValue, 0, 0, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 0, -1, 0, "{\"comments\":[{\"content_offset_seconds\":5}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 0, -1, 0, "{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 0, 4, 0, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 0, 5, 0, "{\"comments\":[{\"content_offset_seconds\":5}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 3, -1, 0, "{\"comments\":[{\"content_offset_seconds\":2}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 3, 5, 0, "{\"comments\":[{\"content_offset_seconds\":2}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 6, -1, 0, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 6, 7, 0, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 5, 5, 0, "{\"comments\":[{\"content_offset_seconds\":0}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 5, 4, 0, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 4, 4, 0, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 6, 6, 0, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 5, -1, 0, "{\"comments\":[{\"content_offset_seconds\":0}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 3, 15, 0, "{\"comments\":[{\"content_offset_seconds\":2}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 3, 17, 0, "{\"comments\":[{\"content_offset_seconds\":2},{\"content_offset_seconds\":14}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 3, -1, 0, "{\"comments\":[{\"content_offset_seconds\":2},{\"content_offset_seconds\":14}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 5, -1, 0, "{\"comments\":[{\"content_offset_seconds\":0},{\"content_offset_seconds\":12}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 5, 16, 0, "{\"comments\":[{\"content_offset_seconds\":0}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 5, 17, 0, "{\"comments\":[{\"content_offset_seconds\":0},{\"content_offset_seconds\":12}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 6, 17, 0, "{\"comments\":[{\"content_offset_seconds\":11}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 6, -1, 0, "{\"comments\":[{\"content_offset_seconds\":11}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 6, 16, 0, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 6, 18, 0, "{\"comments\":[{\"content_offset_seconds\":11}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 17, 17, 0, "{\"comments\":[{\"content_offset_seconds\":0}]}")]
    [InlineData("{\"comments\":[]}", 0, -1, 13, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[]}", 0, long.MinValue, 13, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[]}", long.MaxValue, 0, 13, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 0, -1, 13, "{\"comments\":[{\"content_offset_seconds\":18}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 0, -1, 13, "{\"comments\":[{\"content_offset_seconds\":18},{\"content_offset_seconds\":30}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 0, 4, 13, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 0, 5, 13, "{\"comments\":[{\"content_offset_seconds\":18}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 3, -1, 13, "{\"comments\":[{\"content_offset_seconds\":15}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 3, 5, 13, "{\"comments\":[{\"content_offset_seconds\":15}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 6, -1, 13, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 6, 7, 13, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 5, 5, 13, "{\"comments\":[{\"content_offset_seconds\":13}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 5, 4, 13, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 4, 4, 13, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 6, 6, 13, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5}]}", 5, -1, 13, "{\"comments\":[{\"content_offset_seconds\":13}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 3, 15, 13, "{\"comments\":[{\"content_offset_seconds\":15}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 3, 17, 13, "{\"comments\":[{\"content_offset_seconds\":15},{\"content_offset_seconds\":27}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 3, -1, 13, "{\"comments\":[{\"content_offset_seconds\":15},{\"content_offset_seconds\":27}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 5, -1, 13, "{\"comments\":[{\"content_offset_seconds\":13},{\"content_offset_seconds\":25}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 5, 16, 13, "{\"comments\":[{\"content_offset_seconds\":13}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 5, 17, 13, "{\"comments\":[{\"content_offset_seconds\":13},{\"content_offset_seconds\":25}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 6, 17, 13, "{\"comments\":[{\"content_offset_seconds\":24}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 6, -1, 13, "{\"comments\":[{\"content_offset_seconds\":24}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 6, 16, 13, "{\"comments\":[]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 6, 18, 13, "{\"comments\":[{\"content_offset_seconds\":24}]}")]
    [InlineData("{\"comments\":[{\"content_offset_seconds\":5},{\"content_offset_seconds\":17}]}", 17, 17, 13, "{\"comments\":[{\"content_offset_seconds\":13}]}")]
    public void ApplyOffset_ValidInput(string inputString, long start, long end, long delay, string expectedOutput)
    {
        JToken json = JsonUtils.Deserialize(inputString);

        TransformOptions options = new()
        {
            Start = new(start, true),
            End = new(end, true),
            Delay = new(delay, true),
            SubtitleOptions = DefaultSubtitleOptions
        };
        Transform.ApplyOffset(json, options);
        string output = JsonConvert.SerializeObject(json);

        Assert.Equal(expectedOutput, output);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"comment\":[]}")]
    [InlineData("{\"commentss\":[]}")]
    public void SerializeToPlaintext_NoComments_ThrowsJsonContentExceptionPropertyNotFound(string inputString)
    {
        JToken json = JsonUtils.Deserialize(inputString);

        JsonContentException.PropertyNotFound expectedException = new(string.Empty, "comments");

        void SerializeToPlaintext() => Transform.SerializeToPlaintext(json);

        JsonContentException.PropertyNotFound exception = Assert.Throws<JsonContentException.PropertyNotFound>(SerializeToPlaintext);
        Assert.Equal(expectedException.Message, exception.Message);

        #pragma warning disable CS0162
        return; Transform.SerializeToPlaintext(json);
        #pragma warning restore CS0162
    }

    [Theory]
    [InlineData($"{{\"comments\":[{{{CommenterTemplate},{MessageTemplate}}}]}}")]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{{CommenterTemplate},{MessageTemplate}}}]}}")]
    [InlineData($"{{\"comments\":[{{{CommenterTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}}]}}")]
    public void SerializeToPlaintext_NoContentOffsetSeconds_ThrowsJsonContentExceptionPropertyNotFound(string inputString)
    {
        JToken json = JsonUtils.Deserialize(inputString);

        string expectedPathRegex = @"comments\[\d*\]";
        string expectedPropertyName = "content_offset_seconds";

        void SerializeToPlaintext() => Transform.SerializeToPlaintext(json);

        JsonContentException.PropertyNotFound exception = Assert.Throws<JsonContentException.PropertyNotFound>(SerializeToPlaintext);
        Assert.Matches(expectedPathRegex, exception.Path);
        Assert.Equal(expectedPropertyName, exception.PropertyName);

        #pragma warning disable CS0162
        return; Transform.SerializeToPlaintext(json);
        #pragma warning restore CS0162
    }

    [Theory]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{MessageTemplate}}}]}}")]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{MessageTemplate}}}]}}")]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}}]}}")]
    public void SerializeToPlaintext_NoCommenter_ThrowsJsonContentExceptionPropertyNotFound(string inputString)
    {
        JToken json = JsonUtils.Deserialize(inputString);

        string expectedPathRegex = @"comments\[\d*\]";
        string expectedPropertyName = "commenter";

        void SerializeToPlaintext() => Transform.SerializeToPlaintext(json);

        JsonContentException.PropertyNotFound exception = Assert.Throws<JsonContentException.PropertyNotFound>(SerializeToPlaintext);
        Assert.Matches(expectedPathRegex, exception.Path);
        Assert.Equal(expectedPropertyName, exception.PropertyName);

        #pragma warning disable CS0162
        return; Transform.SerializeToPlaintext(json);
        #pragma warning restore CS0162
    }

    [Theory]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},\"commenter\":{{}},{MessageTemplate}}}]}}")]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},\"commenter\":{{}},{MessageTemplate}}}]}}")]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},\"commenter\":{{}},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}}]}}")]
    public void SerializeToPlaintext_NoDisplayName_ThrowsJsonContentExceptionPropertyNotFound(string inputString)
    {
        JToken json = JsonUtils.Deserialize(inputString);

        string expectedPathRegex = @"comments\[\d*\]\.commenter";
        string expectedPropertyName = "display_name";

        void SerializeToPlaintext() => Transform.SerializeToPlaintext(json);

        JsonContentException.PropertyNotFound exception = Assert.Throws<JsonContentException.PropertyNotFound>(SerializeToPlaintext);
        Assert.Matches(expectedPathRegex, exception.Path);
        Assert.Equal(expectedPropertyName, exception.PropertyName);

        #pragma warning disable CS0162
        return; Transform.SerializeToPlaintext(json);
        #pragma warning restore CS0162
    }

    [Theory]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate}}}]}}")]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate}}}]}}")]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}}]}}")]
    public void SerializeToPlaintext_NoMessage_ThrowsJsonContentExceptionPropertyNotFound(string inputString)
    {
        JToken json = JsonUtils.Deserialize(inputString);

        string expectedPathRegex = @"comments\[\d*\]";
        string expectedPropertyName = "message";

        void SerializeToPlaintext() => Transform.SerializeToPlaintext(json);

        JsonContentException.PropertyNotFound exception = Assert.Throws<JsonContentException.PropertyNotFound>(SerializeToPlaintext);
        Assert.Matches(expectedPathRegex, exception.Path);
        Assert.Equal(expectedPropertyName, exception.PropertyName);

        #pragma warning disable CS0162
        return; Transform.SerializeToPlaintext(json);
        #pragma warning restore CS0162
    }

    [Theory]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},\"message\":{{}}}}]}}")]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},\"message\":{{}}}}]}}")]
    [InlineData($"{{\"comments\":[{{{ContentOffsetSecondsTemplate},{CommenterTemplate},\"message\":{{}}}},{{{ContentOffsetSecondsTemplate},{CommenterTemplate},{MessageTemplate}}}]}}")]
    public void SerializeToPlaintext_NoBody_ThrowsJsonContentExceptionPropertyNotFound(string inputString)
    {
        JToken json = JsonUtils.Deserialize(inputString);

        string expectedPathRegex = @"comments\[\d*\]\.message";
        string expectedPropertyName = "body";

        void SerializeToPlaintext() => Transform.SerializeToPlaintext(json);

        JsonContentException.PropertyNotFound exception = Assert.Throws<JsonContentException.PropertyNotFound>(SerializeToPlaintext);
        Assert.Matches(expectedPathRegex, exception.Path);
        Assert.Equal(expectedPropertyName, exception.PropertyName);

        #pragma warning disable CS0162
        return; Transform.SerializeToPlaintext(json);
        #pragma warning restore CS0162
    }

    // for the plaintext we avoid the raw string literal """ since the interpretation of an end of line will depend on the environment
    // e.g. on Windows in Visual Studio an end of line is interprted as \r\n, but in other systems it may be \n
    // for the actual SerializeToPlaintext method, we prefer just \n and let the OS convert it if appropriate
    [Theory]
    [InlineData(
        """
        {
            "comments": [
                {
                    "content_offset_seconds": 0,
                    "commenter": {
                        "display_name": "JohnSmith"
                    },
                    "message": {
                        "body": "Hello, World!"
                    }
                },
                {
                    "content_offset_seconds": 17,
                    "commenter": {
                        "display_name": "JaneSmith"
                    },
                    "message": {
                        "body": "Hello as well!"
                    }
                },
                {
                    "content_offset_seconds": 7533,
                    "commenter": {
                        "display_name": "JohnSmith"
                    },
                    "message": {
                        "body": "It's been over an hour."
                    }
                },
                {
                    "content_offset_seconds": 93252,
                    "commenter": {
                        "display_name": "JohnSmith"
                    },
                    "message": {
                        "body": "Now it's been over a day."
                    }
                },
                {
                    "content_offset_seconds": 52715391,
                    "commenter": {
                        "display_name": "JohnSmith"
                    },
                    "message": {
                        "body": "Now it's been over a year."
                    }
                }
            ]
        }
        """,
        "00:00:00 JohnSmith: Hello, World!\n" +
        "00:00:17 JaneSmith: Hello as well!\n" +
        "02:05:33 JohnSmith: It's been over an hour.\n" +
        "1.01:54:12 JohnSmith: Now it's been over a day.\n" +
        "610.03:09:51 JohnSmith: Now it's been over a year.\n")]
    public void SerializeToPlaintext_ValidInput(string inputString, string expectedOutput)
    {
        JToken json = JsonUtils.Deserialize(inputString);

        string output = Transform.SerializeToPlaintext(json);

        Assert.Equal(expectedOutput, output);
    }
}