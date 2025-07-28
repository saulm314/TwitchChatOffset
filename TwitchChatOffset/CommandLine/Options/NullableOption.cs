using System;

namespace TwitchChatOffset.CommandLine.Options;

public class NullableOption<TType> : TCOOptionBase<TType> where TType : notnull
{
    public static NullableOption<TType> New(string[] aliases, Func<TType> getDefaultValue, string? description = null)
    {
        ReferenceBool valueSpecified = new(true);
        Func<TType> updatedGetDefaultValue = GetUpdatedGetDefaultValue(getDefaultValue, valueSpecified);
        NullableOption<TType> nullableOption = new(aliases, updatedGetDefaultValue, description, valueSpecified);
        return nullableOption;
    }

    public bool ValueSpecified => valueSpecified.value;

    private NullableOption(string[] aliases, Func<TType> getDefaultValue, string? description, ReferenceBool valueSpecified)
        : base(aliases, getDefaultValue, description)
    {
        this.valueSpecified = valueSpecified;
    }

    private static Func<TType> GetUpdatedGetDefaultValue(Func<TType> getDefaultValue, ReferenceBool valueSpecified)
    {
        TType UpdatedGetDefaultValue()
        {
            valueSpecified.value = false;
            return getDefaultValue();
        }

        return UpdatedGetDefaultValue;
    }

    private readonly ReferenceBool valueSpecified;

    private class ReferenceBool(bool value)
    {
        public bool value = value;
    }
}