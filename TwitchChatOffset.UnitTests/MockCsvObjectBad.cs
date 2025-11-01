using TwitchChatOffset.Options;

namespace TwitchChatOffset.UnitTests;

public class MockCsvObjectBad
{
    public class DuplicateAliasSameField(long? number) : IOptionGroup
    {
        public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(DuplicateAliasSameField));
        private static FieldData[]? _fieldDatas;

        [Aliases(["number", "number"])]
        public long? number = number;

        public DuplicateAliasSameField() : this(default) { }
    }

    public class DuplicateAliasMultipleFields(long? number1, long? number2) : IOptionGroup
    {
        public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(DuplicateAliasMultipleFields));
        private static FieldData[]? _fieldDatas;

        [Aliases(["number1", "number"])]
        public long? number1 = number1;

        [Aliases(["number2", "number"])]
        public long? number2 = number2;

        public DuplicateAliasMultipleFields() : this(default, default) { }
    }

    public class BadGenericNew(long? number) : IOptionGroup
    {
        public static FieldData[] FieldDatas => _fieldDatas ??= IOptionGroup.GetFieldDatas(typeof(BadGenericNew));
        private static FieldData[]? _fieldDatas;

        [Aliases(["number"])]
        public long? number = number;
    }
}