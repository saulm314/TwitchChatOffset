using System;

namespace TwitchChatOffset.CommandLine.Options;

// the purpose of this class is to provide an Option<T> subtype which can also be used as an option in a CSV file
// the option is nullable in the sense that it can distinguish between
//   "no value was specified so the default value was assigned automatically" and "the default value was specified"
// this is done via the bool ValueSpecified property
// although there is no reason T can't be nullable for this behaviour to work correctly, we choose to constrain T to be non-nullable anyway
// this is because in a CSV file, it is impossible to distinguish between a null/empty value and the idea that no value was provided at all
// so instead we assign null the special meaning that no value was provided, and therefore null itself can't be a valid value
// if in the future, a NullableOption<T> is needed that does not parse CSV and does not have this restriction,
//   then the T : notnull constraint can be removed, and a subtype can be created with that constraint, specifically to be used for CSV options
public class NullableOption<T> : TCOOptionBase<T>, INullableOption<T> where T : notnull
{
    public NullableOption(string[] aliases, Func<T> getDefaultValue, string? description = null) : this(aliases, new Data(getDefaultValue), description) { }

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