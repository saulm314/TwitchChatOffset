using TwitchChatOffset.UnitTests;
using Xunit.Sdk;
using System;
using Newtonsoft.Json;

[assembly: RegisterXunitSerializer(typeof(XunitSerializer), typeof(CsvSerializationTests.DeserializeTestData))]
[assembly: RegisterXunitSerializer(typeof(XunitSerializer), typeof(CsvSerializationTests.DeserializeBadTestData))]

namespace TwitchChatOffset.UnitTests;

public class XunitSerializer : IXunitSerializer
{
    public bool IsSerializable(Type type, object? value, out string failureReason)
    {
        failureReason = string.Empty;
        if (type == typeof(CsvSerializationTests.DeserializeTestData))
            return true;
        failureReason = $"Type {type} not supported for serialization";
        return false;
    }

    public object Deserialize(Type type, string serializedValue)
    {
        return JsonConvert.DeserializeObject(serializedValue, type)!;
    }

    public string Serialize(object value)
    {
        return JsonConvert.SerializeObject(value, Formatting.Indented);
    }
}