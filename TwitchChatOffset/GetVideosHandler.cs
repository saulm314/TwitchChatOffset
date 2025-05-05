namespace TwitchChatOffset;

public static class GetVideosHandler
{
    public static void HandleGetVideos(string user, string outputPath, GetVideosFormatting formatting)
    {
        System.Console.WriteLine($"{user} {outputPath} {formatting}");
    }
}