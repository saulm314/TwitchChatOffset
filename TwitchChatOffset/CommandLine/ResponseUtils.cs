using TwitchChatOffset.ConsoleUtils;

namespace TwitchChatOffset.CommandLine;

public static class ResponseUtils
{
    public static Response GetResponseInputOutputWarning(string outputPath)
    {
        string question = $"Warning: output path \"{outputPath}\" is the same as input path, so the input file will get overwritten. Continue?";
        return ConsoleInput.GetResponseWarning(question);
    }

    public static MultiResponse GetMultiResponseInputOutputWarning(string outputPath)
    {
        string question = $"Warning: output path \"{outputPath}\" is the same as input path, so the input file will get overwritten. Continue?";
        return ConsoleInput.GetMultiResponseWarning(question);
    }
}