namespace TwitchChatOffset.Options;

public interface IConflictingOptionGroup : IOptionGroup
{
    long OptionPriority { get; }
}