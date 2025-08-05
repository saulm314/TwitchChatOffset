using System;
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

    public static T? AsN<T>(this JToken jtoken)
    {
        if (typeof(T).IsAssignableTo(typeof(JToken)))
        {
            if (jtoken is not T _jtoken)
                return default;
            return _jtoken;
        }
        if (jtoken is not JValue jvalue)
            return default;
        if (jvalue.Value is not T value)
            return default;
        return value;
    }

    public static bool Is<T>(this JToken jtoken)
    {
        if (typeof(T).IsAssignableTo(typeof(JToken)))
            return jtoken is T;
        if (jtoken is not JValue jvalue)
            return false;
        return jvalue.Value is T;
    }

    public static JToken D(this JToken jtoken, string propertyName)
        => jtoken.As<JObject>()[propertyName] ?? throw new JsonContentException.PropertyNotFound(jtoken.Path, propertyName);

    public static JToken? DN(this JToken jtoken, string propertyName) => jtoken.AsN<JObject>()?[propertyName];

    public static T D<T>(this JToken jtoken, string propertyName) => jtoken.D(propertyName).As<T>();

    public static T? DN<T>(this JToken jtoken, string propertyName)
    {
        JToken? subtoken = jtoken.DN(propertyName);
        if (subtoken == null)
            return default;
        return subtoken.AsN<T>();
    }

    public static bool ContainsProperty(this JToken jtoken, string propertyName) => (jtoken as JObject)?.ContainsKey(propertyName) ?? false;

    public static bool ContainsProperty<T>(this JToken jtoken, string propertyName) => jtoken.DN(propertyName)?.Is<T>() ?? false;

    public static bool ContainsProperty(this JToken jtoken, string propertyName, out JToken? subtoken)
    {
        subtoken = jtoken.DN(propertyName);
        return subtoken != null;
    }

    public static bool ContainsProperty<T>(this JToken jtoken, string propertyName, out T? subtoken)
    {
        subtoken = jtoken.DN<T>(propertyName);
        return subtoken != null;
    }

    public static JToken At(this JToken jtoken, int index)
    {
        JArray jarray = jtoken.As<JArray>();
        if (index < 0 || index >= jarray.Count)
            throw new JsonContentException.OutOfRange(jarray.Path, index);
        return jarray[index];
    }

    public static JToken? AtN(this JToken jtoken, int index)
    {
        if (jtoken is not JArray jarray)
            return null;
        if (index < 0 || index >= jarray.Count)
            return null;
        return jarray[index];
    }

    public static T At<T>(this JToken jtoken, int index) => jtoken.At(index).As<T>();

    public static T? AtN<T>(this JToken jtoken, int index)
    {
        JToken? subtoken = jtoken.AtN(index);
        if (subtoken == null)
            return default;
        return subtoken.AsN<T>();
    }

    public static void SetNull(this JToken jtoken) => jtoken.As<JValue>().Value = null;

    public static bool TrySetNull(this JToken jtoken)
    {
        if (jtoken is not JValue jvalue)
            return false;
        jvalue.Value = null;
        return true;
    }

    public static void Set<T>(this JToken jtoken, T value)
    {
        try
        {
            jtoken.As<JValue>().Value = value;
        }
        catch (ArgumentException)
        {
            throw new JsonContentException.InvalidValueAssignment<T>(jtoken.Path);
        }
    }

    public static bool TrySet<T>(this JToken jtoken, T value)
    {
        if (jtoken is not JValue jvalue)
            return false;
        try
        {
            jvalue.Value = value;
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public static void SetNull(this JToken jtoken, string propertyName) => jtoken.As<JObject>()[propertyName] = JValue.CreateNull();

    public static bool TrySetNull(this JToken jtoken, string propertyName)
    {
        if (jtoken is not JObject jobject)
            return false;
        jobject[propertyName] = JValue.CreateNull();
        return true;
    }

    public static void Set<T>(this JToken jtoken, string propertyName, T value) => jtoken.As<JObject>()[propertyName] = ToJToken(value);

    public static bool TrySet<T>(this JToken jtoken, string propertyName, T value)
    {
        if (jtoken is not JObject jobject)
            return false;
        jobject[propertyName] = ToJToken(value);
        return true;
    }

    public static void SetNull(this JToken jtoken, int index)
    {
        JArray jarray = jtoken.As<JArray>();
        if (index < 0 || index >= jarray.Count)
            throw new JsonContentException.OutOfRange(jarray.Path, index);
        jarray[index] = JValue.CreateNull();
    }

    public static bool TrySetNull(this JToken jtoken, int index)
    {
        if (jtoken is not JArray jarray)
            return false;
        if (index < 0 || index >= jarray.Count)
            return false;
        jarray[index] = JValue.CreateNull();
        return true;
    }

    public static void Set<T>(this JToken jtoken, int index, T value)
    {
        JArray jarray = jtoken.As<JArray>();
        if (index < 0 || index >= jarray.Count)
            throw new JsonContentException.OutOfRange(jarray.Path, index);
        jarray[index] = ToJToken(value);
    }

    public static bool TrySet<T>(this JToken jtoken, int index, T value)
    {
        if (jtoken is not JArray jarray)
            return false;
        if (index < 0 || index >= jarray.Count)
            return false;
        jarray[index] = ToJToken(value);
        return true;
    }

    public static T DeepClone<T>(this JToken jtoken) => jtoken.DeepClone().As<T>();

    public static T? DeepCloneN<T>(this JToken jtoken) => jtoken.DeepClone().AsN<T>();

    public static JToken ToJToken(object? obj)
    {
        if (obj == null)
            return JValue.CreateNull();
        if (obj is JToken jtoken)
            return jtoken;
        try
        {
            return new JValue(obj);
        }
        catch (ArgumentException)
        {
            return JToken.FromObject(obj);
        }
    }

    public static string AddPathWarning(this string message)
        => message + '\n' + "(path may not be the original path if JSON object was modified at the time of the exception being thrown)";

    public static string Dereference(this string path, string propertyName)
        => string.IsNullOrWhiteSpace(path) ? propertyName : path + '.' + propertyName;
}