This is TwitchChatOffset.

The purpose of this software is to take a JSON Twitch chat file, delete all messages up to a particular offset, and off-set all remaining messages by that amount. Also, (optionally) all messages after a desired end point can be deleted.

For example, if you had a JSON Twitch chat file from a VOD that was 3.5 hours long, but you only wanted the chat from the 1-3 hour marks, then this tool will help you take out all the messages from the first hour and all the messages from the last half hour, and then set the 1 hour mark as the "zero" mark for the remaining messages. For example, if there was a message at the 1:00:31 mark, then TwitchChatOffset will convert the time of that message to 0:00:31.

This tool works on Windows only. It may work on other operating systems too, but will likely require installing external software. Look up how to run a .NET executable on your operating system.

# Usage

To use this software, download the latest release and unzip it. From there, in my opinion the easiest way to use it is to take the path of the folder where you unzipped it, and add that path to your PATH environment variable. (Windows key -> "environment variables" -> Edit the system environment variables -> Environment Variables... -> Path (double click) -> New -> [paste your path])

Otherwise, you will have to specify the full path of where you extracted the `TwitchChatOffset.exe` file every time you use the software.

Then, open PowerShell and navigate to the directory with your Twitch chat json file (e.g. `cd D:\TwitchDownloader`). Then simply run `TwitchChatOffset`, specifying the input file, the output file, the start offset in seconds, and optionally the ending time in seconds. For example, if the input file is `twitchChat.json`, the desired start offset is exactly the 1 hour mark, and the desired end point is exactly the 3 hour mark, then you would run `TwitchChatOffset twitchChat.json twitchChatNew.json 3600 10800`. Or, if you didn't add the software to your PATH variable, then you have to specify the exact path of the `TwitchChatOffset.exe` file, e.g. `D:\TwitchChatOffset\TwitchChatOffset.exe twitchChat.json twitchChatNew.json 3600 10800`.

# YouTube Captions

Although this could also be used for other reasons too, the original reason this script was written was to parse messages from a long Twitch VOD into a usable format so that they can be converted to YouTube captions for a video that is only a small part of the Twitch VOD as a whole.

To do this, you can take the following steps:
- Use [Twitch Downloader](https://github.com/lay295/TwitchDownloader) to download the JSON of the Twitch Chat: `TwitchDownloaderCLI -m ChatDownload -u videoID -o videoID.json`
- Use TwitchChatOffset to trim the chat according to your needs: `TwitchChatOffset videoID.json videoIDNew.json 3600 10800`
- Run [this](https://gist.github.com/Cqoicebordel/d9110b4b1191b9e9f6a8165438e00ea0) Python script to convert the JSON file into a YTT file and upload that file as a YouTube subtitle
