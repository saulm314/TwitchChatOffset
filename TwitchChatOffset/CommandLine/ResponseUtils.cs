using TwitchChatOffset.ConsoleUtils;
using System.IO;

namespace TwitchChatOffset.CommandLine;

public static class ResponseUtils
{
    public static Response ValidateInputOutput(ref string inputPath, ref string outputPath, CliResponse cliResponse)
    {
        inputPath = Path.GetRelativePath(".", inputPath);
        outputPath = Path.GetRelativePath(".", outputPath);
        if (inputPath != outputPath)
            return Response.Yes;
        Response response =
            cliResponse == CliResponse.Manual ?
            GetResponseInputOutputWarning(outputPath) :
            (Response)cliResponse;
        if (response == Response.No)
            PrintWarning($"Skipping {outputPath}...", 1);
        else
            PrintWarning($"Overwriting {outputPath}...", 1);
        return response;
    }

    // null means cancel
    public static Response? ValidateInputOutput(ref string inputPath, ref string outputPath, ref MultiResponse? response, CliMultiResponse cliResponse)
    {
        inputPath = Path.GetRelativePath(".", inputPath);
        outputPath = Path.GetRelativePath(".", outputPath);
        if (inputPath != outputPath)
            return Response.Yes;
        if (response == MultiResponse.NoToAll)
        {
            PrintWarning($"Skipping {outputPath}...", 1);
            return Response.No;
        }
        if (response == MultiResponse.YesToAll)
        {
            PrintWarning($"Overwriting {outputPath}...", 1);
            return Response.Yes;
        }
        response =
            cliResponse == CliMultiResponse.Manual ?
            GetMultiResponseInputOutputWarning(outputPath) :
            (MultiResponse)cliResponse;
        if (response == MultiResponse.Cancel)
        {
            PrintError($"Cancelling on {outputPath}...", 2);
            return null;
        }
        if (response == MultiResponse.No || response == MultiResponse.NoToAll)
        {
            PrintWarning($"Skipping {outputPath}...", 1);
            return Response.No;
        }
        PrintWarning($"Overwriting {outputPath}...", 1);
        return Response.Yes;
    }

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