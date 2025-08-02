using TwitchChatOffset.CSV;
using System.Collections.Generic;
using CSVFile;

namespace TwitchChatOffset.UnitTests;

public class CsvSerializationTests
{
    [Theory]
    [MemberData(nameof(GetDeserializeTestData))]
    public void DeserializeTest(DeserializeTestData data)
    {
        CSVReader reader = CSVReader.FromString(data.CsvString, CsvUtils.csvSettings);

        int i = -1;
        foreach (MockCsvObject csvObject in CsvSerialization.Deserialize<MockCsvObject>(reader))
        {
            i++;
            MockCsvObject expectedCsvObject = data.ExpectedCsvObjects[i];

            Assert.Equal(expectedCsvObject, csvObject);
        }
        // make sure the correct number of objects was outputted
        Assert.Equal(i, data.ExpectedCsvObjects.Length - 1);
    }

    public static IEnumerable<TheoryDataRow<DeserializeTestData>> GetDeserializeTestData()
    {
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
    }

    public record DeserializeTestData(string testName, string CsvString, MockCsvObject[] ExpectedCsvObjects);
}