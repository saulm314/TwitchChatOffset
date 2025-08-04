using TwitchChatOffset.CommandLine.Options;

namespace TwitchChatOffset.UnitTests;

public class MockCsvObjectBad
{
    public class DuplicateAliasesSameField(long? number)
    {
        [Aliases(["number", "number"])]
        public long? number = number;

        public DuplicateAliasesSameField() : this(default) { }
    }

    public class DuplicateAliasesMultipleFields(long? number1, long? number2)
    {
        [Aliases(["number1", "number"])]
        public long? number1 = number1;

        [Aliases(["number2", "number"])]
        public long? number2 = number2;

        public DuplicateAliasesMultipleFields() : this(default, default) { }
    }

    public class BadGenericNew(long? number)
    {
        [Aliases(["number"])]
        public long? number = number;
    }
}