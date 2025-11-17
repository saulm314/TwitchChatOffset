using TwitchChatOffset.ConsoleUtils;
using TwitchChatOffset.Options.Groups;
using Newtonsoft.Json.Linq;

namespace TwitchChatOffset.Options.Optimisations;

public class TransformAllData
{
    public Optimisation MaxOptimisation = Optimisation.Same; // if previous transform threw an error, we must not optimise away the step that threw an error
    public bool SkipFile = false;
    public string? InputPath;
    public TransformAllCommonOptions? CommonOptions;
    public MultiResponse? Response;

    public JToken[] OriginalComments = []; // DO NOT modify the JTokens - instead make a deep-clone and modify the deep-clone
    public JToken Json = new JObject();
    public string Output = string.Empty;
}