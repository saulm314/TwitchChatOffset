using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace TwitchChatOffset;

public static class GetVideosHandler
{
    public static void HandleGetVideos(string username, string outputPath, GetVideosFormatting formatting)
    {
        HttpUtils.AuthenticateWithTwitchAsync();
    }
}