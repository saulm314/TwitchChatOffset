using TwitchChatOffset.CommandLine.Options;

namespace TwitchChatOffset.UnitTests;

public class MockCsvObjectBad
{
    public class DuplicateAliasesSameField(long? number)
    {
        [Aliases(["number", "number"])]
        public long? number = number;

        public DuplicateAliasesSameField() : this(default) { }

        public override bool Equals(object? obj)
        {
            if (obj is not DuplicateAliasesSameField other)
                return false;
            return number == other.number;
        }

        public override int GetHashCode() => number.GetHashCode();
    }

    public class DuplicateAliasesMultipleFields(long? number1, long? number2)
    {
        [Aliases(["number1", "number"])]
        public long? number1 = number1;

        [Aliases(["number2", "number"])]
        public long? number2 = number2;

        public DuplicateAliasesMultipleFields() : this(default, default) { }

        private (long?, long?) EqualityFields => (number1, number2);

        public override bool Equals(object? obj)
        {
            if (obj is not DuplicateAliasesMultipleFields other)
                return false;
            return EqualityFields == other.EqualityFields;
        }

        public override int GetHashCode() => EqualityFields.GetHashCode();
    }
}