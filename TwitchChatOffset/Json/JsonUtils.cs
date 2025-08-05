using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset.Json;

public static class JsonUtils
{
    /// <summary>
    /// Deserialize a JSON string into a JObject, or throw JsonContentException if no parent JObject is found
    /// </summary>
    /// <param name="jsonString">the JSON string to deserialize</param>
    /// <returns>the deserialized JObject</returns>
    /// <exception cref="JsonContentException"/>
    public static JObject Deserialize(string jsonString) => (JObject)(JsonConvert.DeserializeObject(jsonString) ?? throw JsonContentException.Empty());

    /// <summary>
    /// If T is a JToken (or derived type), return jtoken but cast to T
    /// If jtoken is a JValue and its value is of type T, return the value
    /// Else throw JsonContentException
    /// </summary>
    /// <typeparam name="T">either a JToken or a valid serializable type as in JValue.GetValueType(JTokenType?, object)</typeparam>
    /// <param name="jtoken">the JToken to convert or get a value of if it's a JValue</param>
    /// <returns>the converted jtoken or the value of the jtoken if it's a JValue</returns>
    /// <exception cref="JsonContentException"/>
    public static T As<T>(this JToken jtoken)
    {
        if (typeof(T).IsAssignableTo(typeof(JToken)))
        {
            if (jtoken is not T _jtoken)
                throw JsonContentException.InvalidConversion<T>(jtoken.Path, jtoken);
            return _jtoken;
        }
        if (jtoken is not JValue jvalue)
            throw JsonContentException.InvalidConversion<T>(jtoken.Path, jtoken);
        if (jvalue.Value is not T value)
            throw JsonContentException.InvalidConversion<T>(jvalue.Path, jvalue.Value);
        return value;
    }

    /// <summary>
    /// Dereference the property of a JObject, and get the JToken representing it, or throw JsonContentException if property not found
    /// E.g. the object.property1.property3 dereference is represented by jobject.D("property1").D("property3")
    /// </summary>
    /// <param name="jtoken">the JObject containing the property to dereference</param>
    /// <param name="propertyName">the name of the property to dereference</param>
    /// <returns>the deferenced JToken</returns>
    /// <exception cref="JsonContentException"/>
    public static JToken D(this JToken jtoken, string propertyName)
        => jtoken.As<JObject>()[propertyName] ?? throw JsonContentException.PropertyNotFound(jtoken.Path, propertyName);

    /// <summary>
    /// Dereference the property of a JObject, and convert it to a JToken subtype, or get its value if the dereferenced JToken is a JValue
    /// </summary>
    /// <typeparam name="T">either a JToken or a valid serializable type as in JValue.GetValueType(JTokenType?, object)</typeparam>
    /// <param name="jtoken">the JObject containing the property to dereference</param>
    /// <param name="propertyName">the name of the property to dereference</param>
    /// <returns>the dereferenced jtoken converted to T if T is a JToken subtype, or the dereferenced value of type T if the dereferenced JToken is a JValue</returns>
    /// <exception cref="JsonContentException"/>
    public static T D<T>(this JToken jtoken, string propertyName) => jtoken.D(propertyName).As<T>();

    /// <summary>
    /// Get the JToken at the specified index of a JArray, or throw ArgumentOutOfRangeException if index is out of range
    /// </summary>
    /// <param name="jtoken">the JArray</param>
    /// <param name="index">index of the desired element</param>
    /// <returns>the JToken representing the element at the specified index</returns>
    /// <exception cref="JsonContentException"/>
    /// <exception cref="System.ArgumentOutOfRangeException"/>
    public static JToken At(this JToken jtoken, int index) => jtoken.As<JArray>()[index];

    /// <summary>
    /// For a specified index of an array, get the element converted to a JToken subtype, or get the element's value, or throw ArgumentOutOfRangeException
    /// </summary>
    /// <typeparam name="T">either a JToken or a valid serializable type as in JValue.GetValueType(JTokenType?, object)</typeparam>
    /// <param name="jtoken">the JArray</param>
    /// <param name="index">index of the desired element</param>
    /// <returns>the element at the specified index, converted to T if T is a JToken subtype, or the value of type T if the element is a JValue</returns>
    /// <exception cref="JsonContentException"/>
    /// <exception cref="System.ArgumentOutOfRangeException"/>
    public static T At<T>(this JToken jtoken, int index) => jtoken.At(index).As<T>();

    /// <summary>
    /// Set the value of a JValue to null
    /// </summary>
    /// <param name="jtoken">the JValue</param>
    /// <exception cref="JsonContentException"/>
    public static void SetNull(this JToken jtoken) => jtoken.As<JValue>().Value = null;

    /// <summary>
    /// Set the value of a JValue
    /// </summary>
    /// <typeparam name="T">a valid serializable type as in JValue.GetValueType(JTokenType?, object)</typeparam>
    /// <param name="jtoken">the JValue</param>
    /// <param name="value">the value to set, possibly null</param>
    /// <exception cref="JsonContentException"/>
    /// <exception cref="System.ArgumentException"/>
    public static void Set<T>(this JToken jtoken, T value) => jtoken.As<JValue>().Value = value;

    /// <summary>
    /// Set a property of a given JObject to the null JValue
    /// </summary>
    /// <param name="jtoken">the JObject</param>
    /// <param name="propertyName">the property to set to null</param>
    /// <exception cref="JsonContentException"/>
    public static void SetNull(this JToken jtoken, string propertyName) => jtoken.As<JObject>()[propertyName] = JValue.CreateNull();

    /// <summary>
    /// Set a property of a given JObject to a given JToken, or a JValue with the given value, or a null JValue
    /// </summary>
    /// <typeparam name="T">either a JToken(?) or a valid serializable type as in JValue.GetValueType(JTokenType?, object)</typeparam>
    /// <param name="jtoken">the JObject</param>
    /// <param name="propertyName">the property to set</param>
    /// <param name="value">the JToken to set, the value to set to the JValue, or null</param>
    /// <exception cref="JsonContentException"/>
    /// <exception cref="System.ArgumentException"/>
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
        jobject[propertyName] = new JValue(value);
    }

    /// <summary>
    /// Set the element at the given index of a JArray to the null JValue, or throw ArgumentOutOfRangeException
    /// </summary>
    /// <param name="jtoken">the JArray</param>
    /// <param name="index">the index at which the element is to be set to null</param>
    /// <exception cref="JsonContentException"/>
    /// <exception cref="System.ArgumentOutOfRangeException"/>
    public static void SetNull(this JToken jtoken, int index) => jtoken.As<JArray>()[index] = JValue.CreateNull();

    /// <summary>
    /// Set the element at a given index of a JArray to a given JToken, or a JValue with the given value, or a null JValue, or throw ArgumentOutOfRangeException
    /// </summary>
    /// <typeparam name="T">either a JToken(?) or a valid serializable type as in JValue.GetValueType(JTokenType?, object)</typeparam>
    /// <param name="jtoken">the JArray</param>
    /// <param name="index">the index at which the element is to be set</param>
    /// <param name="value">the JToken to set, the value to set to the JValue, or null</param>
    /// <exception cref="JsonContentException"/>
    /// <exception cref="System.ArgumentOutOfRangeException"/>
    /// <exception cref="System.ArgumentException"/>
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
        jarray[index] = new JValue(value);
    }

    /// <summary>
    /// Deep clone a JToken and convert it to a JToken subtype or get its value if it's a JValue
    /// </summary>
    /// <typeparam name="T">either a JToken or a valid serializable type as in JValue.GetValueType(JTokenType?, object)</typeparam>
    /// <param name="jtoken">the JToken to deep clone</param>
    /// <returns>the deep clone of the JToken converted to type T, or the value of type T of the deep clone if it's a JValue</returns>
    /// <exception cref="JsonContentException"/>
    public static T DeepClone<T>(this JToken jtoken) => jtoken.DeepClone().As<T>();

    public static string AddPathWarning(this string message)
        => message + '\n' + "(path may not be the original path if JSON object was modified at the time of the exception being thrown)";

    public static string Dereference(this string path, string propertyName)
        => string.IsNullOrWhiteSpace(path) ? propertyName : path + '.' + propertyName;
}