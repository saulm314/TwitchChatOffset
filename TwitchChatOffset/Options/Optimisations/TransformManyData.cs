using TwitchChatOffset.ConsoleUtils;
using TwitchChatOffset.Options.Groups;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset.Options.Optimisations;

public class TransformManyData
{
    public TransformManyCommonOptions? CommonOptions;
    public MultiResponse? Response;

    // do not modify the contents of these (can modify the references though)
    // if you need to modify, make a copy first
    public JToken[]? OriginalComments;
    public JToken? EmptyJson;
    public JToken? FilledJson;
    public string? Output;
}