using TwitchChatOffset.Csv;
using System;
using System.Collections.Generic;
using System.Linq;
using CSVFile;
using Newtonsoft.Json;

namespace TwitchChatOffset.UnitTests;

public class CsvSerializationTests
{
    [Theory]
    [MemberData(nameof(GetDeserializeTestData))]
    public void DeserializeTest(DeserializeTestData data)
    {
        using CSVReader reader1 = CSVReader.FromString(data.CsvString, CsvUtils.csvSettings);
        using CSVReader reader2 = CSVReader.FromString(data.CsvString, CsvUtils.csvSettings);

        int i = -1;
        foreach (MockCsvObject csvObject in CsvSerialization.Deserialize<MockCsvObject>(reader1))
        {
            i++;
            MockCsvObject expectedCsvObject = data.ExpectedCsvObjects[i];

            Assert.Equal(expectedCsvObject, csvObject);
        }
        // make sure the correct number of objects was outputted
        Assert.Equal(i, data.ExpectedCsvObjects.Length - 1);

        i = -1;
        foreach (MockCsvObject csvObject in CsvSerialization.Deserialize(reader2, typeof(MockCsvObject)))
        {
            i++;
            MockCsvObject expectedCsvObject = data.ExpectedCsvObjects[i];

            Assert.Equal(expectedCsvObject, csvObject);
        }
        // make sure the correct number of objects was outputted
        Assert.Equal(i, data.ExpectedCsvObjects.Length - 1);
    }

    [Theory]
    [MemberData(nameof(GetDeserializeBadTestData))]
    public void Deserialize_DuplicateHeader_ThrowsCsvContentExceptionDuplicateOption(DeserializeBadTestData data)
    {
        using CSVReader reader1 = CSVReader.FromString(data.CsvString, CsvUtils.csvSettings);
        using CSVReader reader2 = CSVReader.FromString(data.CsvString, CsvUtils.csvSettings);

        void Deserialize1() => _ = CsvSerialization.Deserialize<MockCsvObject>(reader1).Count();
        void Deserialize2() => _ = ((IEnumerable<object>)CsvSerialization.Deserialize(reader2, typeof(MockCsvObject))).Count();

        CsvContentException exception1 = Assert.Throws<CsvContentException>(Deserialize1);
        CsvContentException exception2 = Assert.Throws<CsvContentException>(Deserialize2);
        Assert.Equal(data.ExpectedException.Message, exception1.Message);
        Assert.Equal(data.ExpectedException.Message, exception2.Message);

        #pragma warning disable CS0162
        return;
        _ = CsvSerialization.Deserialize<MockCsvObject>(reader1).Count();
        _ = ((IEnumerable<object>)CsvSerialization.Deserialize(reader2, typeof(MockCsvObject))).Count();
        #pragma warning restore CS0162
    }

    [Theory]
    [MemberData(nameof(GetDeserializeBadInternalTestData))]
    public void Deserialize_DuplicateFields_ThrowsCsvSerializationInternalExceptionDuplicateAliases(DeserializeBadInternalTestData data)
    {
        using CSVReader reader1 = CSVReader.FromString(string.Empty);
        using CSVReader reader2 = CSVReader.FromString(string.Empty);

        void Deserialize1() => _ = data.GenericDeserialize(reader1).Count();
        void Deserialize2() => _ = ((IEnumerable<object>)CsvSerialization.Deserialize(reader2, data.PType)).Count();

        CsvSerializationInternalException exception1 = Assert.Throws<CsvSerializationInternalException>(Deserialize1);
        CsvSerializationInternalException exception2 = Assert.Throws<CsvSerializationInternalException>(Deserialize2);
        Assert.Equal(data.ExpectedException.Message, exception1.Message);
        Assert.Equal(data.ExpectedException.Message, exception2.Message);

        #pragma warning disable CS0162
        return;
        _ = CsvSerialization.Deserialize<MockCsvObjectBad.DuplicateAliasesSameField>(reader1).Count();
        _ = ((IEnumerable<object>)CsvSerialization.Deserialize(reader2, data.PType)).Count();
        #pragma warning restore CS0162
    }

    [Fact]
    public void Deserialize_BadGenericNew_Throws()
    {
        using CSVReader reader = CSVReader.FromString(string.Empty);
        Type type = typeof(MockCsvObjectBad.BadGenericNew);

        void Deserialize() => _ = ((IEnumerable<object>)CsvSerialization.Deserialize(reader, type)).Count();

        Assert.ThrowsAny<Exception>(Deserialize);

        #pragma warning disable CS0162
        return; _ = ((IEnumerable<object>)CsvSerialization.Deserialize(reader, type)).Count();
        #pragma warning restore CS0162
    }

    public static IEnumerable<TheoryDataRow<DeserializeTestData>> GetDeserializeTestData()
    {
        yield return new(new
        (
            "empty string",
            """

            """,
            []
        ));

        yield return new(new
        (
            "empty header",
            """
            hello
            """,
            []
        ));

        yield return new(new
        (
            "two extra empty headers",
            """
            hello,hello2
            """,
            []
        ));

        yield return new(new
        (
            "empty extra headers with no data",
            """
            hello,hello2
            ,

            """,
            [
                new()
            ]
        ));

        yield return new(new
        (
            "too many commas",
            """
            hello,hello2
            ,,

            """,
            [
                new()
            ]
        ));

        yield return new(new
        (
            "way too many commas",
            """
            hello,hello2
            ,,,,,,,,,

            """,
            [
                new()
            ]
        ));

        yield return new(new
        (
            "empty extra headers with no data (two rows)",
            """
            hello,hello2
            ,
            ,
            """,
            [
                new(),
                new()
            ]
        ));

        yield return new(new
        (
            "empty real headers with no data",
            """
            long-object-default,long-object-stripped
            ,

            """,
            [
                new(),
            ]
        ));

        yield return new(new
        (
            "missing data",
            """
            long-object-default,long-object-stripped
            5,5
            5
            ,

            5

            """,
            [
                new(5, 5, default, default, default, default, default, default, default),
                new(5, default, default, default, default, default, default, default, default),
                new(),
                new(),
                new(5, default, default, default, default, default, default, default, default)
            ]
        ));

        yield return new(new
        (
            "general test, exhausting all edge cases of all types",
            """
            long-object-default,long-object-stripped,--long-object-unstripped,long-object-non-nullable,bool-object,char-object,double-object,mock-enum-object,string-object
            ,,,,,,,,
            0,0,0,0,false,a,0.0,PascalCase,Hello World!
            1,2,3,4,true,A,1.2,camelCase,Goodbye!
            -1,-2,-3,-4,False,0,-0.0,snake_case,Goodbye!
            123456,234567,345678,456789,True, ,-1.2,pascalcase,Goodbye!
            123456,234567,345678,456789,True, ,12345.6,CAMELCASE,Goodbye!
            123456,234567,345678,456789,True, ,-12345.6,SnAkE_cAsE,Goodbye!
            123456,234567,345678,456789,True, ,0,SnAkE_cAsE,Goodbye!
            123456,234567,345678,456789,True, ,5,SnAkE_cAsE,Goodbye!
            123456,234567,345678,456789,True, ,-5,SnAkE_cAsE,Goodbye!
            hello,51.0,true, ,yes,two,hello,snakecase,1
            5a,a5,0.0,5f,no,three,5.0f,snakecase,1
            """,
            [
                new(null, null, null, 0, null, null, null, null, null),
                new(0, 0, 0, 0, false, 'a', 0.0, MockEnum.PascalCase, "Hello World!"),
                new(1, 2, 3, 4, true, 'A', 1.2, MockEnum.camelCase, "Goodbye!"),
                new(-1, -2, -3, -4, false, '0', -0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -1.2, MockEnum.PascalCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 12345.6, MockEnum.camelCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -12345.6, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 5.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -5.0, MockEnum.snake_case, "Goodbye!"),
                new(null, null, null, 0, null, null, null, null, "1"),
                new(null, null, null, 0, null, null, null, null, "1")
            ]
        ));

        yield return new(new
        (
            """
            extra header should be ignored in final output
            long-object-unstripped header should be ignored in final output, since it doesn't match any alias (alias is supposed to be unstripped)
                 that is, it should have "--" in the beginning, but the correct alias --long-object-unstripped is still counted
            """,
            """
            extra,long-object-default,long-object-unstripped,long-object-stripped,--long-object-unstripped,long-object-non-nullable,bool-object,char-object,double-object,mock-enum-object,string-object
            19,,17,,,,,,,,
            19,0,17,0,0,0,false,a,0.0,PascalCase,Hello World!
            19,1,17,2,3,4,true,A,1.2,camelCase,Goodbye!
            19,-1,17,-2,-3,-4,False,0,-0.0,snake_case,Goodbye!
            19,123456,17,234567,345678,456789,True, ,-1.2,pascalcase,Goodbye!
            19,123456,17,234567,345678,456789,True, ,12345.6,CAMELCASE,Goodbye!
            19,123456,17,234567,345678,456789,True, ,-12345.6,SnAkE_cAsE,Goodbye!
            19,123456,17,234567,345678,456789,True, ,0,SnAkE_cAsE,Goodbye!
            19,123456,17,234567,345678,456789,True, ,5,SnAkE_cAsE,Goodbye!
            19,123456,17,234567,345678,456789,True, ,-5,SnAkE_cAsE,Goodbye!
            19,hello,17,51.0,true, ,yes,two,hello,snakecase,1
            19,5a,17,a5,0.0,5f,no,three,5.0f,snakecase,1
            """,
            [
                new(null, null, null, 0, null, null, null, null, null),
                new(0, 0, 0, 0, false, 'a', 0.0, MockEnum.PascalCase, "Hello World!"),
                new(1, 2, 3, 4, true, 'A', 1.2, MockEnum.camelCase, "Goodbye!"),
                new(-1, -2, -3, -4, false, '0', -0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -1.2, MockEnum.PascalCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 12345.6, MockEnum.camelCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -12345.6, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 5.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -5.0, MockEnum.snake_case, "Goodbye!"),
                new(null, null, null, 0, null, null, null, null, "1"),
                new(null, null, null, 0, null, null, null, null, "1")
            ]
        ));

        yield return new(new
        (
            "--long-object-stripped and long-object-unstripped (both wrong aliases) both ignored, thus all values are default for those two headers",
            """
            long-object-default,--long-object-stripped,long-object-unstripped,long-object-non-nullable,bool-object,char-object,double-object,mock-enum-object,string-object
            ,,,,,,,,
            0,0,0,0,false,a,0.0,PascalCase,Hello World!
            1,2,3,4,true,A,1.2,camelCase,Goodbye!
            -1,-2,-3,-4,False,0,-0.0,snake_case,Goodbye!
            123456,234567,345678,456789,True, ,-1.2,pascalcase,Goodbye!
            123456,234567,345678,456789,True, ,12345.6,CAMELCASE,Goodbye!
            123456,234567,345678,456789,True, ,-12345.6,SnAkE_cAsE,Goodbye!
            123456,234567,345678,456789,True, ,0,SnAkE_cAsE,Goodbye!
            123456,234567,345678,456789,True, ,5,SnAkE_cAsE,Goodbye!
            123456,234567,345678,456789,True, ,-5,SnAkE_cAsE,Goodbye!
            hello,51.0,true, ,yes,two,hello,snakecase,1
            5a,a5,0.0,5f,no,three,5.0f,snakecase,1
            """,
            [
                new(null, null, null, 0, null, null, null, null, null),
                new(0, null, null, 0, false, 'a', 0.0, MockEnum.PascalCase, "Hello World!"),
                new(1, null, null, 4, true, 'A', 1.2, MockEnum.camelCase, "Goodbye!"),
                new(-1, null, null, -4, false, '0', -0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, null, null, 456789, true, ' ', -1.2, MockEnum.PascalCase, "Goodbye!"),
                new(123456, null, null, 456789, true, ' ', 12345.6, MockEnum.camelCase, "Goodbye!"),
                new(123456, null, null, 456789, true, ' ', -12345.6, MockEnum.snake_case, "Goodbye!"),
                new(123456, null, null, 456789, true, ' ', 0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, null, null, 456789, true, ' ', 5.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, null, null, 456789, true, ' ', -5.0, MockEnum.snake_case, "Goodbye!"),
                new(null, null, null, 0, null, null, null, null, "1"),
                new(null, null, null, 0, null, null, null, null, "1")
            ]
        ));

        yield return new(new
        (
            "two extras with the same name, both ignored",
            """
            extra,extra,long-object-default,long-object-unstripped,long-object-stripped,--long-object-unstripped,long-object-non-nullable,bool-object,char-object,double-object,mock-enum-object,string-object
            19,20,,17,,,,,,,,
            19,20,0,17,0,0,0,false,a,0.0,PascalCase,Hello World!
            19,20,1,17,2,3,4,true,A,1.2,camelCase,Goodbye!
            19,20,-1,17,-2,-3,-4,False,0,-0.0,snake_case,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-1.2,pascalcase,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,12345.6,CAMELCASE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-12345.6,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,0,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,5,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-5,SnAkE_cAsE,Goodbye!
            19,20,hello,17,51.0,true, ,yes,two,hello,snakecase,1
            19,20,5a,17,a5,0.0,5f,no,three,5.0f,snakecase,1
            """,
            [
                new(null, null, null, 0, null, null, null, null, null),
                new(0, 0, 0, 0, false, 'a', 0.0, MockEnum.PascalCase, "Hello World!"),
                new(1, 2, 3, 4, true, 'A', 1.2, MockEnum.camelCase, "Goodbye!"),
                new(-1, -2, -3, -4, false, '0', -0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -1.2, MockEnum.PascalCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 12345.6, MockEnum.camelCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -12345.6, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 5.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -5.0, MockEnum.snake_case, "Goodbye!"),
                new(null, null, null, 0, null, null, null, null, "1"),
                new(null, null, null, 0, null, null, null, null, "1")
            ]
        ));

        yield return new(new
        (
            """
            two long-object-unstripped headers (supposed to have "--") - both ignored (--long-object-unstripped also present)
            --long-object-stripped (supposed to be stripped, that is, no "--" in the beginning), should be ignored (long-object-stripped also present)
            """,
            """
            long-object-unstripped,--long-object-stripped,long-object-default,long-object-unstripped,long-object-stripped,--long-object-unstripped,long-object-non-nullable,bool-object,char-object,double-object,mock-enum-object,string-object
            19,20,,17,,,,,,,,
            19,20,0,17,0,0,0,false,a,0.0,PascalCase,Hello World!
            19,20,1,17,2,3,4,true,A,1.2,camelCase,Goodbye!
            19,20,-1,17,-2,-3,-4,False,0,-0.0,snake_case,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-1.2,pascalcase,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,12345.6,CAMELCASE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-12345.6,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,0,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,5,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-5,SnAkE_cAsE,Goodbye!
            19,20,hello,17,51.0,true, ,yes,two,hello,snakecase,1
            19,20,5a,17,a5,0.0,5f,no,three,5.0f,snakecase,1
            """,
            [
                new(null, null, null, 0, null, null, null, null, null),
                new(0, 0, 0, 0, false, 'a', 0.0, MockEnum.PascalCase, "Hello World!"),
                new(1, 2, 3, 4, true, 'A', 1.2, MockEnum.camelCase, "Goodbye!"),
                new(-1, -2, -3, -4, false, '0', -0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -1.2, MockEnum.PascalCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 12345.6, MockEnum.camelCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -12345.6, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 5.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -5.0, MockEnum.snake_case, "Goodbye!"),
                new(null, null, null, 0, null, null, null, null, "1"),
                new(null, null, null, 0, null, null, null, null, "1")
            ]
        ));

        yield return new(new
        (
            "second aliases general test",
            """
            long-object-unstripped,--long-object-stripped,longObjectDefault,long-object-unstripped,long-stripped,--long-unstripped,long-non-nullable,bool,char,double,mock-enum,string
            19,20,,17,,,,,,,,
            19,20,0,17,0,0,0,false,a,0.0,PascalCase,Hello World!
            19,20,1,17,2,3,4,true,A,1.2,camelCase,Goodbye!
            19,20,-1,17,-2,-3,-4,False,0,-0.0,snake_case,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-1.2,pascalcase,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,12345.6,CAMELCASE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-12345.6,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,0,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,5,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-5,SnAkE_cAsE,Goodbye!
            19,20,hello,17,51.0,true, ,yes,two,hello,snakecase,1
            19,20,5a,17,a5,0.0,5f,no,three,5.0f,snakecase,1
            """,
            [
                new(null, null, null, 0, null, null, null, null, null),
                new(0, 0, 0, 0, false, 'a', 0.0, MockEnum.PascalCase, "Hello World!"),
                new(1, 2, 3, 4, true, 'A', 1.2, MockEnum.camelCase, "Goodbye!"),
                new(-1, -2, -3, -4, false, '0', -0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -1.2, MockEnum.PascalCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 12345.6, MockEnum.camelCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -12345.6, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 5.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -5.0, MockEnum.snake_case, "Goodbye!"),
                new(null, null, null, 0, null, null, null, null, "1"),
                new(null, null, null, 0, null, null, null, null, "1")
            ]
        ));

        yield return new(new
        (
            "second aliases general test, including false stripping second aliases",
            """
            long-unstripped,--long-stripped,longObjectDefault,long-unstripped,long-stripped,--long-unstripped,long-non-nullable,bool,char,double,mock-enum,string
            19,20,,17,,,,,,,,
            19,20,0,17,0,0,0,false,a,0.0,PascalCase,Hello World!
            19,20,1,17,2,3,4,true,A,1.2,camelCase,Goodbye!
            19,20,-1,17,-2,-3,-4,False,0,-0.0,snake_case,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-1.2,pascalcase,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,12345.6,CAMELCASE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-12345.6,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,0,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,5,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-5,SnAkE_cAsE,Goodbye!
            19,20,hello,17,51.0,true, ,yes,two,hello,snakecase,1
            19,20,5a,17,a5,0.0,5f,no,three,5.0f,snakecase,1
            """,
            [
                new(null, null, null, 0, null, null, null, null, null),
                new(0, 0, 0, 0, false, 'a', 0.0, MockEnum.PascalCase, "Hello World!"),
                new(1, 2, 3, 4, true, 'A', 1.2, MockEnum.camelCase, "Goodbye!"),
                new(-1, -2, -3, -4, false, '0', -0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -1.2, MockEnum.PascalCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 12345.6, MockEnum.camelCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -12345.6, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 5.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -5.0, MockEnum.snake_case, "Goodbye!"),
                new(null, null, null, 0, null, null, null, null, "1"),
                new(null, null, null, 0, null, null, null, null, "1")
            ]
        ));

        yield return new(new
        (
            "different order",
            """
            string,long-unstripped,--long-stripped,longObjectDefault,long-unstripped,long-stripped,--long-unstripped,long-non-nullable,bool,char,double,mock-enum
            ,19,20,,17,,,,,,,
            Hello World!,19,20,0,17,0,0,0,false,a,0.0,PascalCase
            Goodbye!,19,20,1,17,2,3,4,true,A,1.2,camelCase
            Goodbye!,19,20,-1,17,-2,-3,-4,False,0,-0.0,snake_case
            Goodbye!,19,20,123456,17,234567,345678,456789,True, ,-1.2,pascalcase
            Goodbye!,19,20,123456,17,234567,345678,456789,True, ,12345.6,CAMELCASE
            Goodbye!,19,20,123456,17,234567,345678,456789,True, ,-12345.6,SnAkE_cAsE
            Goodbye!,19,20,123456,17,234567,345678,456789,True, ,0,SnAkE_cAsE
            Goodbye!,19,20,123456,17,234567,345678,456789,True, ,5,SnAkE_cAsE
            Goodbye!,19,20,123456,17,234567,345678,456789,True, ,-5,SnAkE_cAsE
            1,19,20,hello,17,51.0,true, ,yes,two,hello,snakecase
            1,19,20,5a,17,a5,0.0,5f,no,three,5.0f,snakecase
            """,
            [
                new(null, null, null, 0, null, null, null, null, null),
                new(0, 0, 0, 0, false, 'a', 0.0, MockEnum.PascalCase, "Hello World!"),
                new(1, 2, 3, 4, true, 'A', 1.2, MockEnum.camelCase, "Goodbye!"),
                new(-1, -2, -3, -4, false, '0', -0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -1.2, MockEnum.PascalCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 12345.6, MockEnum.camelCase, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -12345.6, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 0.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', 5.0, MockEnum.snake_case, "Goodbye!"),
                new(123456, 234567, 345678, 456789, true, ' ', -5.0, MockEnum.snake_case, "Goodbye!"),
                new(null, null, null, 0, null, null, null, null, "1"),
                new(null, null, null, 0, null, null, null, null, "1")
            ]
        ));
    }

    public static IEnumerable<TheoryDataRow<DeserializeBadTestData>> GetDeserializeBadTestData()
    {
        yield return new(new
        (
            "duplicate long-object-default",
            """
            long-object-default,long-object-default
            """,
            CsvContentException.DuplicateOption("long-object-default")
        ));

        yield return new(new
        (
            "duplicate long-object-default with lines",
            """
            long-object-default,long-object-default



            """,
            CsvContentException.DuplicateOption("long-object-default")
        ));

        yield return new(new
        (
            "duplicate long-object-default with data",
            """
            long-object-default,long-object-default
            1,2
            5,1
            6,7
            """,
            CsvContentException.DuplicateOption("long-object-default")
        ));

        yield return new(new
        (
            "duplicate long-object-default longObjectDefault",
            """
            long-object-default,longObjectDefault
            """,
            CsvContentException.DuplicateOption("longObjectDefault")
        ));

        yield return new(new
        (
            "duplicate long-object-default longObjectDefault with lines",
            """
            long-object-default,longObjectDefault



            """,
            CsvContentException.DuplicateOption("longObjectDefault")
        ));

        yield return new(new
        (
            "duplicate long-object-default longObjectDefault with data",
            """
            long-object-default,longObjectDefault
            1,2
            5,1
            6,7
            """,
            CsvContentException.DuplicateOption("longObjectDefault")
        ));

        yield return new(new
        (
            "duplicate big (different aliases)",
            """
            long-unstripped,--long-stripped,longObjectDefault,long-unstripped,long-stripped,--long-unstripped,long-non-nullable,bool,char,double,mock-enum,string,long-object-stripped
            19,20,,17,,,,,,,,
            19,20,0,17,0,0,0,false,a,0.0,PascalCase,Hello World!
            19,20,1,17,2,3,4,true,A,1.2,camelCase,Goodbye!
            19,20,-1,17,-2,-3,-4,False,0,-0.0,snake_case,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-1.2,pascalcase,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,12345.6,CAMELCASE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-12345.6,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,0,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,5,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-5,SnAkE_cAsE,Goodbye!
            19,20,hello,17,51.0,true, ,yes,two,hello,snakecase,1
            19,20,5a,17,a5,0.0,5f,no,three,5.0f,snakecase,1
            """,
            CsvContentException.DuplicateOption("long-object-stripped")
        ));

        yield return new(new
        (
            "duplicate big (same aliases)",
            """
            long-unstripped,--long-stripped,longObjectDefault,long-unstripped,long-stripped,--long-unstripped,long-non-nullable,bool,char,double,mock-enum,string,long-stripped
            19,20,,17,,,,,,,,
            19,20,0,17,0,0,0,false,a,0.0,PascalCase,Hello World!
            19,20,1,17,2,3,4,true,A,1.2,camelCase,Goodbye!
            19,20,-1,17,-2,-3,-4,False,0,-0.0,snake_case,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-1.2,pascalcase,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,12345.6,CAMELCASE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-12345.6,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,0,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,5,SnAkE_cAsE,Goodbye!
            19,20,123456,17,234567,345678,456789,True, ,-5,SnAkE_cAsE,Goodbye!
            19,20,hello,17,51.0,true, ,yes,two,hello,snakecase,1
            19,20,5a,17,a5,0.0,5f,no,three,5.0f,snakecase,1
            """,
            CsvContentException.DuplicateOption("long-stripped")
        ));
    }

    public static IEnumerable<TheoryDataRow<DeserializeBadInternalTestData>> GetDeserializeBadInternalTestData()
    {
        yield return new(new
        (
            nameof(MockCsvObjectBad.DuplicateAliasesSameField),
            CsvSerialization.Deserialize<MockCsvObjectBad.DuplicateAliasesSameField>,
            typeof(MockCsvObjectBad.DuplicateAliasesSameField),
            CsvSerializationInternalException.DuplicateAliases("number", typeof(MockCsvObjectBad.DuplicateAliasesSameField))
        ));

        yield return new(new
        (
            nameof(MockCsvObjectBad.DuplicateAliasesMultipleFields),
            CsvSerialization.Deserialize<MockCsvObjectBad.DuplicateAliasesMultipleFields>,
            typeof(MockCsvObjectBad.DuplicateAliasesMultipleFields),
            CsvSerializationInternalException.DuplicateAliases("number", typeof(MockCsvObjectBad.DuplicateAliasesMultipleFields))
        ));
    }

    public record DeserializeTestData(string TestName, string CsvString, MockCsvObject[] ExpectedCsvObjects);

    public record DeserializeBadTestData(string TestName, string CsvString, CsvContentException ExpectedException);

    // Newtonsoft.Json cannot serialize delegates correctly, so we specify that it should be ignored
    // else all tests that use this as its underlying type in the MemberDataAttribute will show up as a single test
    public record DeserializeBadInternalTestData(string TestName, [property: JsonIgnore] Func<CSVReader, IEnumerable<object>> GenericDeserialize, Type PType,
        CsvSerializationInternalException ExpectedException);
}