using System;
using System.ComponentModel;
using System.Diagnostics;

namespace TwitchChatOffset;

public static class GetVideosHandler
{
    public static void HandleGetVideos(string user, string outputPath, GetVideosFormatting formatting)
    {
        Authenticate();
    }

    private const string AuthUrl =
        "https://id.twitch.tv/oauth2/authorize?response_type=token&client_id=wt92v33cpoj7s7ccx0u3s1klz1oadl&redirect_uri=https://localhost:32100";

    private static void Authenticate()
    {
        Console.WriteLine("Opening browser...");
        try
        {
            Process process = new();
            ProcessStartInfo startInfo = new(AuthUrl)
            {
                UseShellExecute = true
            };
            process.StartInfo = startInfo;
            process.Start();
        }
        catch (Win32Exception) { }
        catch (PlatformNotSupportedException) { }
        Console.WriteLine($"If browser failed to open, then open it manually and copy-and-paste the following URL:\n{AuthUrl}");
    }
}