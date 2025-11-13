using System;

namespace TwitchChatOffset.ConsoleUtils;

public static class ConsoleInput
{
    public static Response GetResponse(string question)
    {
        PrintLine(question);
        return GetResponse();
    }

    public static Response GetResponseWarning(string question)
    {
        PrintWarning(question);
        return GetResponse();
    }

    public static Response GetResponseError(string question)
    {
        PrintError(question);
        return GetResponse();
    }

    public static MultiResponse GetMultiResponse(string question)
    {
        PrintLine(question);
        return GetMultiResponse();
    }

    public static MultiResponse GetMultiResponseWarning(string question)
    {
        PrintWarning(question);
        return GetMultiResponse();
    }

    public static MultiResponse GetMultiResponseError(string question)
    {
        PrintError(question);
        return GetMultiResponse();
    }

    private static Response GetResponse()
    {
        Response? response = null;
        while (response == null)
            response = GetResponseOrNull();
        return (Response)response;
    }

    private static Response? GetResponseOrNull()
    {
        PrintLine("[y] yes  [n] no");
        Print("> ");
        string? responseStr = Console.ReadLine()?.Trim().ToLower();
        Response? response = responseStr switch
        {
            "y" => Response.Yes,
            "n" => Response.No,
            _ => null
        };
        if (response == null)
            PrintError("Invalid response; try again");
        return response;
    }

    private static MultiResponse GetMultiResponse()
    {
        MultiResponse? response = null;
        while (response == null)
            response = GetMultiResponseOrNull();
        return (MultiResponse)response;
    }

    private static MultiResponse? GetMultiResponseOrNull()
    {
        PrintLine("[y] yes  [a] yes-to-all  [n] no  [l] no-to-all  [c] cancel");
        Print("> ");
        string? responseStr = Console.ReadLine()?.Trim().ToLower();
        MultiResponse? response = responseStr switch
        {
            "y" => MultiResponse.Yes,
            "a" => MultiResponse.YesToAll,
            "n" => MultiResponse.No,
            "l" => MultiResponse.NoToAll,
            "c" => MultiResponse.Cancel,
            _ => null
        };
        if (response == null)
            PrintError("Invalid response; try again");
        return response;
    }
}