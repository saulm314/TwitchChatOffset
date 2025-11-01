using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using TwitchChatOffset.Options.Groups;

namespace TwitchChatOffset.Options;

public sealed class OptionGroupBox<TOptionGroup>(TOptionGroup value) where TOptionGroup : struct, IOptionGroup
{
    public readonly TOptionGroup Value = value;

    public static readonly FieldInfo ValueField = typeof(OptionGroupBox<TOptionGroup>).GetField(nameof(Value), BindingFlags.Instance | BindingFlags.Public)!;
}

public static class OptionGroupBox
{
    public static FieldInfo GetValueField(Type type)
        => typeof(OptionGroupBox<TransformOptions>)
        .GetGenericTypeDefinition()
        .MakeGenericType(type)
        .GetField(nameof(OptionGroupBox<TransformOptions>.Value), BindingFlags.Instance | BindingFlags.Public)!;
}