using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset.Json;

public static class JsonUtils
{
    public static JObject Deserialize(string jsonString) => (JObject)(JsonConvert.DeserializeObject(jsonString) ?? throw new JsonContentException.Empty());

    public static T As<T>(this JToken jtoken)
    {
        if (typeof(T).IsAssignableTo(typeof(JToken)))
        {
            if (jtoken is not T _jtoken)
                throw new JsonContentException.InvalidConversion<T>(jtoken.Path, jtoken);
            return _jtoken;
        }
        if (jtoken is not JValue jvalue)
            throw new JsonContentException.InvalidConversion<T>(jtoken.Path, jtoken);
        if (jvalue.Value is not T value)
            throw new JsonContentException.InvalidConversion<T>(jvalue.Path, jvalue.Value);
        return value;
    }

    public static JToken D(this JToken jtoken, string propertyName)
        => jtoken.As<JObject>()[propertyName] ?? throw new JsonContentException.PropertyNotFound(jtoken.Path, propertyName);

    public static T D<T>(this JToken jtoken, string propertyName) => jtoken.D(propertyName).As<T>();

    public static JToken At(this JToken jtoken, int index) => jtoken.As<JArray>()[index];

    public static T At<T>(this JToken jtoken, int index) => jtoken.At(index).As<T>();

    public static void SetNull(this JToken jtoken) => jtoken.As<JValue>().Value = null;

    public static void Set<T>(this JToken jtoken, T value) => jtoken.As<JValue>().Value = value;

    public static void SetNull(this JToken jtoken, string propertyName) => jtoken.As<JObject>()[propertyName] = JValue.CreateNull();

    public static void Set<T>(this JToken jtoken, string propertyName, T value)
    {
        JObject jobject = jtoken.As<JObject>();
        if (value == null)
        {
            jobject[propertyName] = JValue.CreateNull();
            return;
        }
        if (value is JToken _jtoken)
        {
            jobject[propertyName] = _jtoken;
            return;
        }
        if (jobject[propertyName] is JValue jvalue)
        {
            jvalue.Value = value;
            return;
        }
        jobject[propertyName] = new JValue(value);
    }

    public static void SetNull(this JToken jtoken, int index) => jtoken.As<JArray>()[index] = JValue.CreateNull();

    public static void Set<T>(this JToken jtoken, int index, T value)
    {
        JArray jarray = jtoken.As<JArray>();
        if (value == null)
        {
            jarray[index] = JValue.CreateNull();
            return;
        }
        if (value is JToken _jtoken)
        {
            jarray[index] = _jtoken;
            return;
        }
        if (jarray[index] is JValue jvalue)
        {
            jvalue.Value = value;
        }
        jarray[index] = new JValue(value);
    }

    public static T DeepClone<T>(this JToken jtoken) => jtoken.DeepClone().As<T>();

    public static string AddPathWarning(this string message)
        => message + '\n' + "(path may not be the original path if JSON object was modified at the time of the exception being thrown)";

    public static string Dereference(this string path, string propertyName)
        => string.IsNullOrWhiteSpace(path) ? propertyName : path + '.' + propertyName;
}