using TwitchChatOffset.UnitTests;
using Xunit.Sdk;
using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

// for any Theory tests that use the MemberDataAttribute, the underlying type must be either serializable by default
//      or explicitly added through this assembly attribute
// otherwise the test explorer will not display the tests correctly (multiple tests will show as one)
[assembly: RegisterXunitSerializer(typeof(XunitSerializer),
    typeof(CsvSerializationTests.DeserializeTestData),
    typeof(CsvSerializationTests.DeserializeBadTestData),
    typeof(CsvSerializationTests.DeserializeBadInternalTestData))]

namespace TwitchChatOffset.UnitTests;

public class XunitSerializer : IXunitSerializer
{
    private static readonly RegisterXunitSerializerAttribute? attribute
        = Assembly.GetExecutingAssembly().GetCustomAttribute<RegisterXunitSerializerAttribute>();

    public bool IsSerializable(Type type, object? value, out string failureReason)
    {
        failureReason = string.Empty;
        if (attribute != null && attribute.SupportedTypesForSerialization.Contains(type))
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