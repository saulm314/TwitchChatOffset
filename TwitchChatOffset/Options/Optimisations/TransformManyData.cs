using TwitchChatOffset.ConsoleUtils;
using TwitchChatOffset.Options.Groups;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset.Options.Optimisations;

public class TransformManyData
{
    public TransformManyCommonOptions? CommonOptions;
    public MultiResponse? Response;

    public JToken[]? OriginalComments; // DO NOT modify the JTokens - instead make a deep-clone and modify the deep-clone
    public JToken? Json;
    public string? Output;
}