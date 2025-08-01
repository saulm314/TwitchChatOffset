using System;

namespace TwitchChatOffset.CommandLine.Options;

public class NullableOption<T> : TCOOptionBase<T>, INullableOption<T> where T : notnull
{
    public NullableOption(string[] aliases, Func<T> getDefaultValue, string? description) : this(aliases, new Data(getDefaultValue), description) { }

    public bool ValueSpecified => data.valueSpecified;

    private NullableOption(string[] aliases, Data data, string? description) : base(aliases, data.UpdatedGetDefaultValue, description) => this.data = data;

    private readonly Data data;

    private class Data(Func<T> getDefaultValue)
    {
        public T UpdatedGetDefaultValue()
        {
            valueSpecified = true;
            return getDefaultValue();
        }

        public bool valueSpecified = false;
    }
}