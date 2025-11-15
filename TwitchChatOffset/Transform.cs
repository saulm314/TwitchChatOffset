using TwitchChatOffset.Json;
using TwitchChatOffset.Options.Groups;
using TwitchChatOffset.Ytt;
using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public static class Transform
{
    public static string DoTransform(string inputString, TransformCommonOptions options)
    {
        JObject json = JsonUtils.Deserialize(inputString);
        ApplyOffset(json, options);
        return Serialize(json, options);
    }

    public static string DoTransform(JToken inputJson, TransformCommonOptions options)
    {
        JToken json = inputJson.DeepClone();
        ApplyOffset(json, options);
        return Serialize(json, options);
    }

    //_______________________________________________________________________

    public static (JToken[], JToken) GetSortedOriginalCommentsAndJson(string input)
    {
        JToken json = JsonUtils.Deserialize(input);
        JArray commentsJArray = json.D("comments").As<JArray>();
        JToken[] comments = [..commentsJArray];
        comments.Sort(CommentComparer.Instance);
        return (comments, json);
    }

    // allComments must be sorted
    private static readonly JToken _startTemplate = new JObject() { ["content_offset_seconds"] = JsonUtils.ToJToken((long)0) };
    private static readonly JToken _endTemplate = new JObject() { ["content_offset_seconds"] = JsonUtils.ToJToken((long)0) };
    public static void ApplyOffset(JToken[] allComments, JToken json, TransformCommonOptions options)
    {
        (long start, long end, long delay) = options;
        if (delay < 0)
        {
            PrintWarning("Warning: delay value is less than zero which is not supported; treating it as zero instead");
            delay = options.Delay.Value = 0;
        }
        if (end >= 0 && end < start)
        {
            PrintWarning("Warning: end value is less than start value, so all comments will get deleted");
            json.Set("comments", new JArray());
            return;
        }
        // if there is no offset to apply compared to the original JSON, we still have to re-add all the values manually,
        // since we are assuming that the original JSON file was unsorted
        JArray selectedComments = [];
        json.Set("comments", selectedComments);
        _startTemplate.Set("content_offset_seconds", start);
        _endTemplate.Set("content_offset_seconds", end);
        int startIndex = GetIndex(allComments, _startTemplate);
        int endIndex = end >= 0 ? GetIndex(allComments, _endTemplate) : allComments.Length;
        for (int i = startIndex; i < endIndex && i < allComments.Length; i++)
        {
            JToken comment = allComments[i].DeepClone();
            JValue offsetJValue = comment.D("content_offset_seconds").As<JValue>();
            long offset = offsetJValue.As<long>();
            offsetJValue.Set(offset - start + delay);
            selectedComments.Add(comment);
        }
    }

    private static int GetIndex(JToken[] allComments, JToken template)
    {
        int index = Array.BinarySearch(allComments, template, CommentComparer.Instance);
        if (index < 0)
            return ~index;
        int lastIndex = index;
        for (int i = index - 1; i >= 0; i--)
        {
            if (CommentComparer.Instance.Compare(allComments[i], template) != 0)
                break;
            lastIndex = i;
        }
        return lastIndex;
    }

    public static void ApplyOffset(JToken json, TransformCommonOptions options)
    {
        (long start, long end, long delay) = options;
        if (delay < 0)
        {
            PrintWarning("Warning: delay value is less than zero which is not supported; treating it as zero instead");
            delay = 0;
        }
        if (end >= 0 && end < start)
            PrintWarning("Warning: end value is less than start value, so all comments will get deleted");
        if (start == 0 && end < 0 && delay == 0)
            return;
        JArray comments = json.D("comments").As<JArray>();
        int i = 0;
        while (i < comments.Count)
        {
            JValue offsetJValue = comments[i].D("content_offset_seconds").As<JValue>();
            long offset = offsetJValue.As<long>();
            bool inRange = offset >= start && (offset <= end || end < 0);
            if (!inRange)
            {
                comments.RemoveAt(i);
                continue;
            }
            offsetJValue.Set(offset - start + delay);
            i++;
        }
    }

    public static string Serialize(JToken json, TransformCommonOptions options)
    {
        return options.Format.Value switch
        {
            Format.Json => SerializeToJson(json),
            Format.JsonIndented => SerializeToJsonIndented(json),
            Format.Ytt => SerializeToYtt(json, options.SubtitleOptions),
            Format.Plaintext => SerializeToPlaintext(json),
            _ => throw new InternalException("Internal error: unrecognised format type")
        };
    }

    //_____________________________________________________________________

    public static string SerializeToJson(JToken json)
    {
        return JsonConvert.SerializeObject(json);
    }

    public static string SerializeToJsonIndented(JToken json)
    {
        return JsonConvert.SerializeObject(json, Formatting.Indented);
    }

    public static string SerializeToYtt(JToken json, SubtitleOptions options)
    {
        return YttSerialization.Serialize(json, options);
    }

    public static string SerializeToPlaintext(JToken json)
    {
        StringBuilder builder = new();
        JArray comments = json.D("comments").As<JArray>();
        foreach (JToken comment in comments)
        {
            long offset = comment.D("content_offset_seconds").As<long>();
            TimeSpan timeSpan = TimeSpan.FromSeconds(offset);
            string displayName = comment.D("commenter").D("display_name").As<string>();
            string message = comment.D("message").D("body").As<string>();

            builder.Append(timeSpan);
            builder.Append(' ');
            builder.Append(displayName);
            builder.Append(": ");
            builder.Append(message);
            builder.Append('\n');
        }
        return builder.ToString();
    }
}