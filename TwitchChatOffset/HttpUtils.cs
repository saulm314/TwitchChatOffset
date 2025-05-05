using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace TwitchChatOffset;

public static class HttpUtils
{
    public static readonly HttpClient client = new();

    private const ushort Port = 32100;

    // For the OAuth2 redirect URI, we can use HTTP instead of HTTPS since we only connect to localhost,
    // hence the request never leaves the user's machine and does not need encryption
    // For more info see: https://www.oauth.com/oauth2-servers/oauth-native-apps/redirect-urls-for-native-apps/ (Loopback URLs)

    public static void AuthenticateWithTwitch()
    {
        Console.WriteLine("Authenticating with Twitch...");
        using HttpListener listener = new();
        listener.Prefixes.Add(redirectUri);
        listener.Start();
        OpenBrowser();
        HttpListenerContext context = listener.GetContext();
        HttpListenerRequest request = context.Request;
        Console.WriteLine(request.RawUrl);
    }

    private static readonly string redirectUri = $"http://localhost:{Port}/";
    private static readonly string authUrl =
        $"https://id.twitch.tv/oauth2/authorize?response_type=token&client_id=wt92v33cpoj7s7ccx0u3s1klz1oadl&redirect_uri={redirectUri}";

    private static void OpenBrowser()
    {
        Console.WriteLine("Opening browser...");
        try
        {
            Process process = new();
            ProcessStartInfo startInfo = new(authUrl)
            {
                UseShellExecute = true
            };
            process.StartInfo = startInfo;
            process.Start();
        }
        catch (Win32Exception) { }
        catch (PlatformNotSupportedException) { }
        Console.WriteLine($"If browser failed to open, then open it manually and copy-and-paste the following URL:\n{authUrl}");
    }

}