/*using System.Collections.Generic;

namespace TwitchChatOffset.UnitTests;

public class CsvSerializationTests
{
    [Theory]
    [MemberData(nameof(GetDeserializeTestData))]
    public void DeserializeTest(DeserializeTestData data)
    {

    }

    public static IEnumerable<TheoryDataRow<DeserializeTestData>> GetDeserializeTestData()
    {
        yield return new
        (
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
        );
        yield return new
        (
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
        );
        yield return new
        (
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
        );
    }

    public record DeserializeTestData(string CsvString, MockCsvObject[] ExpectedCsvObjects);
}*/