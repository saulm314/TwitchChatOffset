using TwitchChatOffset.Json;
using TwitchChatOffset.Options.Groups;
using TwitchChatOffset.Subtitles;
using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset;

public static class Transform
{
    public static string DoTransform(string input, TransformCommonOptions options)
    {
        (JToken[] allComments, JToken json) = GetSortedOriginalCommentsAndJson(input);
        ApplyOffset(allComments, json, options);
        return Serialize(json, options);
    }

    //_______________________________________________________________________

    public static (JToken[], JToken) GetSortedOriginalCommentsAndJson(string input)
    {
        JToken json = JsonUtils.Deserialize(input);
        JArray commentsJArray = json.D("comments").As<JArray>();

        // use OrderBy over Sort for stable sort
        JToken[] comments = [..commentsJArray.OrderBy(comment => comment.D("content_offset_seconds").As<long>())];

        // if array has length 1, the OrderBy method does nothing
        // so we manually check if content_offset_seconds exists and throw exception if not
        if (comments.Length == 1)
            _ = comments[0].D("content_offset_seconds").As<long>();

        return (comments, json);
    }

    // allComments must be sorted
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
        int startIndex = GetStartIndex(allComments);
        int endIndex = end >= 0 ? GetEndIndex(allComments) : allComments.Length;
        for (int i = startIndex; i < endIndex && i < allComments.Length; i++)
        {
            JToken comment = allComments[i].DeepClone();
            JValue offsetJValue = comment.D("content_offset_seconds").As<JValue>();
            long offset = offsetJValue.As<long>();
            offsetJValue.Set(offset - start + delay);
            selectedComments.Add(comment);
        }
    }

    private static readonly JToken _startTemplate = new JObject() { ["content_offset_seconds"] = JsonUtils.ToJToken((long)0) };
    private static int GetStartIndex(JToken[] allComments)
    {
        int index = Array.BinarySearch(allComments, _startTemplate, CommentComparer.Instance);
        if (index < 0)
            return ~index;
        int firstIndex = index;
        for (int i = index - 1; i >= 0; i--)
        {
            if (CommentComparer.Instance.Compare(allComments[i], _startTemplate) != 0)
                break;
            firstIndex = i;
        }
        return firstIndex;
    }

    private static readonly JToken _endTemplate = new JObject() { ["content_offset_seconds"] = JsonUtils.ToJToken((long)0) };
    private static int GetEndIndex(JToken[] allComments)
    {
        int index = Array.BinarySearch(allComments, _endTemplate, CommentComparer.Instance);
        if (index < 0)
            return ~index;
        int lastIndex = index;
        for (int i = index + 1; i < allComments.Length; i++)
        {
            if (CommentComparer.Instance.Compare(allComments[i], _endTemplate) != 0)
                break;
            lastIndex = i;
        }
        return lastIndex + 1;
    }

    public static string Serialize(JToken json, TransformCommonOptions options)
    {
        return options.Format.Value switch
        {
            Format.Json => SerializeToJson(json),
            Format.JsonIndented => SerializeToJsonIndented(json),
            Format.Ytt => SerializeToYtt(json, options.SubtitleOptions, options.Format),
            Format.Ass => SerializeToAss(json, options.SubtitleOptions, options.Format),
            Format.Plaintext => SerializeToPlaintext(json),
            _ => throw new InternalException($"Internal error: unrecognised format type {options.Format.Value}")
        };
    }

    //_____________________________________________________________________

    public static string SerializeToJson(JToken json)
    {
        return JsonUtils.Serialize(json);
    }

    public static string SerializeToJsonIndented(JToken json)
    {
        return JsonUtils.Serialize(json, Formatting.Indented);
    }

    public static string SerializeToYtt(JToken json, SubtitleOptions options, Format format)
    {
        return SubtitleSerialization.Serialize(json, options, format);
    }

    public static string SerializeToAss(JToken json, SubtitleOptions options, Format format)
    {
        return SubtitleSerialization.Serialize(json, options, format);
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