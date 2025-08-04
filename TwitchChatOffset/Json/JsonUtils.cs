using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset.Json;

public static class JsonUtils
{
    public static JToken Deserialize(string jsonString) => (JToken)(JsonConvert.DeserializeObject(jsonString) ?? throw JsonContentException.Empty());

    // D for Dereference
    // i.e. exampleObject.field1.field3 is acquired by token.D("field1").D("field3")
    public static JToken D(this JToken jtoken, string propertyName)
        => jtoken[propertyName] ?? throw JsonContentException.PropertyNotFound($"{jtoken.Path}.{propertyName}");

    public static T D<T>(this JToken jtoken, string propertyName)
        => jtoken.D(propertyName).ToObject<T>() ?? throw JsonContentException.InvalidConversion<T>($"{jtoken.Path}.{propertyName}");

    public static T Value<T>(this JToken jtoken)
        => jtoken.ToObject<T>() ?? throw JsonContentException.InvalidConversion<T>(jtoken.Path);
}