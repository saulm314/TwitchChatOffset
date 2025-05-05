using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TwitchChatOffset;

public static class HttpUtils
{
    public static readonly HttpClient client = new();

    public static void AuthenticateWithTwitch()
    {
        Console.WriteLine("Authenticating with Twitch...");
        using HttpListener listener = new();
        listener.Prefixes.Add("http://localhost:32100/");
        listener.Start();
        OpenBrowser();
        HttpListenerContext context = listener.GetContext();
        HttpListenerRequest request = context.Request;
        Console.WriteLine(request.RawUrl);
    }

    private const string AuthUrl =
        "https://id.twitch.tv/oauth2/authorize?response_type=token&client_id=wt92v33cpoj7s7ccx0u3s1klz1oadl&redirect_uri=https://localhost:32100";

    private static void OpenBrowser()
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