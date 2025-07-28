using TwitchChatOffset;
using TwitchChatOffset.CSV;

namespace TwitchChatOffsetUnitTests;

public class BulkTransformTests
{
    private static TransformManyToManyCsvNullables[] AllMtmNullableses =>
        [
            new(),
            new()
            {
                inputFile = "chat.json",
                outputFile = "transformedChat.json",
                start = 0,
                end = 0,
                format = Format.Json,
                outputDir = ".",
                optionPriority = 0
            },
            new()
            {
                inputFile = "chat.json",
                outputFile = "transformedChat.json",
                start = 1,
                end = 1,
                format = Format.JsonIndented,
                outputDir = ".",
                optionPriority = 1
            },
            new()
            {
                inputFile = "chat.json",
                outputFile = "transformedChat.json",
                start = 10,
                end = 10,
                format = Format.Plaintext,
                outputDir = ".",
                optionPriority = 10
            },
            new()
            {
                inputFile = "chat.json",
                outputFile = "transformedChat.json",
                start = 100,
                end = 100,
                format = Format.Json,
                outputDir = ".",
                optionPriority = 100
            },
            new()
            {
                inputFile = "chat.json",
                outputFile = "transformedChat.json",
                start = 1000,
                end = 1000,
                format = Format.Json,
                outputDir = ".",
                optionPriority = 1000
            },
            new()
            {
                inputFile = "chat.json",
                outputFile = "transformedChat.json",
                start = -1,
                end = -1,
                format = Format.Json,
                outputDir = ".",
                optionPriority = -1
            },
            new()
            {
                inputFile = "chat.json",
                outputFile = "transformedChat.json",
                start = -10,
                end = -10,
                format = Format.Json,
                outputDir = ".",
                optionPriority = -10
            },
            new()
            {
                inputFile = "chat.json",
                outputFile = "transformedChat.json",
                start = -100,
                end = -100,
                format = Format.Json,
                outputDir = ".",
                optionPriority = -100
            },
            new()
            {
                inputFile = "chat.json",
                outputFile = "transformedChat.json",
                start = -1000,
                end = -1000,
                format = Format.Json,
                outputDir = ".",
                optionPriority = -1000
            },
            new()
            {
                inputFile = "chat.json",
                outputFile = "transformedChat.json",
                start = long.MinValue,
                end = long.MinValue,
                format = Format.Json,
                outputDir = ".",
                optionPriority = long.MinValue
            },
            new()
            {
                inputFile = "chat.json",
                outputFile = "transformedChat.json",
                start = long.MaxValue,
                end = long.MaxValue,
                format = Format.Json,
                outputDir = ".",
                optionPriority = long.MaxValue
            },
        ];

    private static TransformOneToManyCsvNullables[] AllOtmNullableses =>
        [
            new(),
            new()
            {
                outputFile = "transformedChat.json",
                start = 0,
                end = 0,
                format = Format.Json,
                outputDir = ".",
                optionPriority = 0
            },
            new()
            {
                outputFile = "transformedChat.json",
                start = 1,
                end = 1,
                format = Format.JsonIndented,
                outputDir = ".",
                optionPriority = 1
            },
            new()
            {
                outputFile = "transformedChat.json",
                start = 10,
                end = 10,
                format = Format.Plaintext,
                outputDir = ".",
                optionPriority = 10
            },
            new()
            {
                outputFile = "transformedChat.json",
                start = 100,
                end = 100,
                format = Format.Json,
                outputDir = ".",
                optionPriority = 100
            },
            new()
            {
                outputFile = "transformedChat.json",
                start = 1000,
                end = 1000,
                format = Format.Json,
                outputDir = ".",
                optionPriority = 1000
            },
            new()
            {
                outputFile = "transformedChat.json",
                start = -1,
                end = -1,
                format = Format.Json,
                outputDir = ".",
                optionPriority = -1
            },
            new()
            {
                outputFile = "transformedChat.json",
                start = -10,
                end = -10,
                format = Format.Json,
                outputDir = ".",
                optionPriority = -10
            },
            new()
            {
                outputFile = "transformedChat.json",
                start = -100,
                end = -100,
                format = Format.Json,
                outputDir = ".",
                optionPriority = -100
            },
            new()
            {
                outputFile = "transformedChat.json",
                start = -1000,
                end = -1000,
                format = Format.Json,
                outputDir = ".",
                optionPriority = -1000
            },
            new()
            {
                outputFile = "transformedChat.json",
                start = long.MinValue,
                end = long.MinValue,
                format = Format.Json,
                outputDir = ".",
                optionPriority = long.MinValue
            },
            new()
            {
                outputFile = "transformedChat.json",
                start = long.MaxValue,
                end = long.MaxValue,
                format = Format.Json,
                outputDir = ".",
                optionPriority = long.MaxValue
            },
        ];

    private static MockNullableOption<long>[] AllStartEndOptions =>
        [
            new(0, false), new(0, true),
            new(1, false), new(1, true),
            new(10, false), new(10, true),
            new(100, false), new(100, true),
            new(1000, false), new(1000, true),
            new(-1, false), new(-1, true),
            new(-10, false), new(-10, true),
            new(-100, false), new(-100, true),
            new(-1000, false), new(-1000, false),
            new(long.MinValue, false), new(long.MinValue, true),
            new(long.MaxValue, false), new(long.MaxValue, true)
        ];

    private static MockNullableOption<Format>[] AllFormatOptions =>
        [
            new(Format.Json, false), new(Format.Json, true),
            new(Format.JsonIndented, false), new(Format.JsonIndented, true),
            new(Format.Plaintext, false), new(Format.Plaintext, true)
        ];

    private static MockNullableOption<string>[] AllOutputDirOptions =>
        [
            new(".", false), new(".", true)
        ];

    private static long[] AllOptionPriorities => [0, 1, 10, 100, 1000, -1, -10, -100, -1000, long.MinValue, long.MaxValue];

    [Fact]
    public void TryGetNonNullableLine_InputFileNull_ReturnsNull()
    {
        TransformManyToManyCsvNullables[] nullableses = [.. AllMtmNullableses.Transform(nullables => nullables.inputFile = null)];
        MockNullableOption<long>[] starts = AllStartEndOptions;
        MockNullableOption<long>[] ends = AllStartEndOptions;
        MockNullableOption<Format>[] formats = AllFormatOptions;
        MockNullableOption<string>[] outputDirs = AllOutputDirOptions;
        long[] optionPriorities = AllOptionPriorities;

        int maxLength = ListUtils.MaxLength(nullableses, starts, ends, formats, outputDirs, optionPriorities);
        for (int i = 0; i < maxLength; i++)
        {
            var nullables = nullableses.IthOrLast(i);
            var start = starts.IthOrLast(i);
            var end = ends.IthOrLast(i);
            var format = formats.IthOrLast(i);
            var outputDir = outputDirs.IthOrLast(i);
            var optionPriority = optionPriorities.IthOrLast(i);

            TransformManyToManyCsv? output = BulkTransform.TryGetNonNullableLine(nullables, start, end, format, outputDir, optionPriority);

            Assert.Null(output);
        }
    }

    [Fact]
    public void TryGetNonNullableLine_OutputFileNull_ReturnsNull()
    {
        TransformManyToManyCsvNullables[] mtmNullableses = [..AllMtmNullableses.Transform(nullables => nullables.outputFile = null)];
        TransformOneToManyCsvNullables[] otmNullableses = [..AllOtmNullableses.Transform(nullables => nullables.outputFile = null)];
        MockNullableOption<long>[] starts = AllStartEndOptions;
        MockNullableOption<long>[] ends = AllStartEndOptions;
        MockNullableOption<Format>[] formats = AllFormatOptions;
        MockNullableOption<string>[] outputDirs = AllOutputDirOptions;
        long[] optionPriorities = AllOptionPriorities;

        int maxLength = ListUtils.MaxLength(mtmNullableses, otmNullableses, starts, ends, formats, outputDirs, optionPriorities);
        for (int i = 0; i < maxLength; i++)
        {
            var mtmNullables = mtmNullableses.IthOrLast(i);
            var otmNullables = otmNullableses.IthOrLast(i);
            var start = starts.IthOrLast(i);
            var end = ends.IthOrLast(i);
            var format = formats.IthOrLast(i);
            var outputDir = outputDirs.IthOrLast(i);
            var optionPriority = optionPriorities.IthOrLast(i);

            TransformManyToManyCsv? output1 = BulkTransform.TryGetNonNullableLine(mtmNullables, start, end, format, outputDir, optionPriority);
            TransformOneToManyCsv? output2 = BulkTransform.TryGetNonNullableLine(otmNullables, start, end, format, outputDir, optionPriority);

            Assert.Null(output1);
            Assert.Null(output2);
        }
    }
}