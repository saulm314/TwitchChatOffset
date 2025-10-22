# TwitchChatOffset
This is TwitchChatOffset.

The purpose of this software is to take a JSON Twitch chat file, apply various transformations to it, and serialize it back into one of various formats (JSON, YTT, plaintext). The primary uses are for cropping/delaying Twitch chats and for converting JSON Twitch chat files into YTT YouTube subtitle files, so that the Twitch chat can be replayed directly in YouTube, as seen [here](https://youtu.be/y7_ZrMJNHUk).

TwitchChatOffset also supports bulk transformations, so if you have many Twitch chats (or one big Twitch chat that needs to split into several videos), you can put the details of each chat into a CSV table and TwitchChatOffset will automatically apply all the transformations into as many files as needed.

The app has been tested quite extensively with over 150 unit tests, but if you find a bug feel free to raise an issue. To handle the YTT conversions, the app uses [YTSubConverter](https://github.com/arcusmaximus/YTSubConverter).

TwitchChatOffset can be run on any operating system that supports the .NET Runtime (Windows, MacOS, Linux), though you may need to install .NET manually if you don't have it already.

TwitchChatOffset is available as a console application only.
# Installation
To install the app, first download the latest release [here](https://github.com/saulm314/TwitchChatOffset/releases), and extract the ZIP file.

Then, to run the app, open a terminal and navigate to the folder where you extracted the ZIP file, and run `.\TwitchChatOffset.exe` on Windows or `dotnet TwitchChatOffset.dll` on any operating system. If it doesn't work, you likely don't have the .NET runtime installed. The required .NET version is 9+, so you can download it [here](https://dotnet.microsoft.com/en-us/download/dotnet/9.0/runtime) and once you have it installed try running the app again.

If you are on Windows, it is recommended to use [PowerShell](https://github.com/PowerShell/PowerShell/releases) rather than the default terminal app, since the default terminal may not be able to parse the command inputs correctly. In particular, the default terminal does not parse `""` as an empty string but rather it thinks no input was provided at all.

To see what commands are available, run `TwitchChatOffset --help` (or `.\TwitchChatOffset.exe --help` or `dotnet TwitchChatOffset.dll --help`).
## For Novices
If you aren't using Windows, you're probably not a novice so this section assumes you use Windows.

Once you open a terminal (PowerShell is preferred), you can navigate to the directory of the app by running `cd path\to\app` where `path\to\app` represents the directory where you extracted the app. For example, it may be `cd C:\Users\JohnSmith\Desktop\TwitchChatOffset`.

Then, run `.\TwitchChatOffset.exe` and if it says something like "Required command was not provided" and gives you a list of commands, then you've run the app successfully.

Although you can simply run the app like this, it is recommended to add it to your PATH environment variable so that you can run it from any directory. Do this by pressing the Windows key and searching for "environment variables", then _Edit the system environment variables_ -> _Environment Variables..._ -> _Path_ (double click) -> _New_ -> _[paste your path, same as `path\to\app` from before]_ -> _[click OK / save everything]_.

If you've done this correctly, then you can close and reopen your terminal, and simply run `TwitchChatOffset`, and you should see the "Required command was not provided" output even if you didn't navigate to the directory where you extracted the app.
# Acquiring The Twitch Chat JSON File
To download a Twitch chat JSON file to your local machine, you can use [TwitchDownloader](https://github.com/lay295/TwitchDownloader/releases).

Then, go to the Twitch video for which you want the chat, and get the video ID from the URL. E.g., if the video URL is `https://www.twitch.tv/videos/4815162342`, then the video ID is `4815162342`.

Then, to download the Twitch chat for this video, run:
```
TwitchDownloaderCLI ChatDownload -u videoID -o videoID.json
```
and replace `videoID` with the ID obtained in the previous step, e.g.:
```
TwitchDownloaderCLI ChatDownload -u 4815162342 -o 4815162342.json
```
# Usage
## Transform
Apply some transformations to a Twitch chat JSON file and output the contents into another file.
```
TwitchChatOffset transform <input-path> <output-path> [options]
```
To see more information about each argument and option, run `TwitchChatOffset transform --help`.
### Examples
Aim: to take a JSON Twitch chat file `chat.json`, cut out all the chat before the 1 hour mark, cut out all the chat after the 3 hour mark, and add 1 minute of delay before the first chat starts, then put the contents in another JSON file with human-readable indentations `transformed-chat.json`. E.g. if the first message after the 1 hour mark was at the 1:00:21 mark, we now want it to be at the 0:01:21 mark.
```
TwitchChatOffset transform chat.json transformed-chat.json --start 3600 --end 10800 --delay 60 -f jsonindented
```
Aim: to take a JSON Twitch chat file `chat.json` and apply the same transformations as previously, but this time to convert it into a text file `transformed-chat.txt` where the chat is displayed in human-readable form.
```
TwitchChatOffset transform chat.json transformed-chat.txt --start 3600 --end 10800 --delay 60 -f plaintext
```
Aim: as above but this time we're putting the entire chat (including before the 1 hour mark and after the 3 hour mark) into a `transformed-full-chat.txt` file.
```
TwitchChatOffset transform chat.json transformed-full-chat.txt -f plaintext
```
Aim: Same as the first example, but this time we want to convert it to a YouTube subtitle YTT file `transformed-chat.ytt` where the subtitles are displayed in the top-right corner.
```
TwitchChatOffset transform chat.json transformed-chat.ytt --start 3600 --end 10800 --delay 60 -f ytt --ytt-position topright
```
## Transform All
Find all files that have a name with a matching search pattern in a given directory, then apply a transformation to each of them, and create a new output file for each input file.
```
TwitchChatOffset transform-all <suffix> [options]
```
To see more information about each argument and option, run `TwitchChatOffset transform-all --help`.
### Examples
Aim: to find all JSON files in the current directory, cut out the first minute of their chats, and store the resulting chats in new JSON files in the `transformed` directory, with each output file having the same name as its corresponding input file.
```
TwitchChatOffset transform-all ".json" -o transformed --start 60
```
Aim: as before but this time we want to leave the output files in the same directory, but adding `-transformed` to their names to differentiate them from the input files. E.g. `chat.json` becomes `chat-transformed.json`.
```
TwitchChatOffset transform-all "-transformed.json" --start 60
```
Aim: same as the previous example, but this time we only want to apply transformations to JSON files that have "`chat`" in their names.
```
TwitchChatOffset transform-all "-transformed.json" --start 60 --pattern "*chat*.json"
```
## Transform Many to Many
Using a CSV file with a list of input JSON files and transformations, apply all the listed transformations and create an output file for each. For the CSV file, use the template provided [here](templates/transform-many-to-many-template.csv).
```
TwitchChatOffset transform-many-to-many <csv-path> [options]
```
To see more information about each argument and option, run `TwitchChatOffset transform-many-to-many --help`.

Most of the options that are available on the command line are also available in the CSV file with the same alias but without leading hyphens. For example, the `--ytt-position` command-line option is also available as a CSV option as `ytt-position`. An exhaustive list of all available options in the CSV file is defined [here](TwitchChatOffset/Csv/TransformManyToManyCsvNullables.cs).

The same option can be specified both in the CSV file and on the command line. In that case, the program uses the value from the CSV file and ignores the one from the command line. A field or an entire column can also be left blank in the CSV file. In that case, for that file, the value provided on the command line is used instead. If no value is provided on the command line, then the hardcoded default value is used (run the `--help` option to see default values).

However, this behaviour can be overridden using the `option-priority` option. (WARNING: this is complicated so it is recommended to leave this option alone unless you have a very specific use case.) `option-priority` is available both in CLI (command-line interface) and CSV, and it can be any integer. If this option is omitted either on the CLI or the CSV, it defaults to 0. When the option priority is the same on both the CLI and the CSV, the behaviour is as described above. However, when the CLI option priority is strictly greater than the CSV option priority, then CLI values will be prioritised over CSV values. The table below illustrates this behaviour:

|                              | CLI Option Not Specified | CLI Option Specified        |
| ---------------------------- | ------------------------ | --------------------------- |
| **CSV Option Not Specified** | hardcoded value is used  | CLI value is used           |
| **CSV Option Specified**     | CSV value is used        | special value is calculated |

The special value means that if the CSV option priority is equal to or greater than the CLI option priority, then the CSV value is used; else the CLI value is used.
### Examples
Aim: transform all files in `transform-many-to-many.csv`, such that if for any file no `ytt-position` is specified, default it to bottom-right.
```
TwitchChatOffset transform-many-to-many transform-many-to-many.csv --ytt-position bottomright
```
## Transform One To Many
Using a JSON Twitch chat file and a CSV file with a list of transformations, apply all the listed transformations and create an output file for each. For the CSV file, use the template provided [here](templates/transform-one-to-many-template.csv).
```
TwitchChatOffset transform-one-to-many <input-path> <csv-path> [options]
```
To see more information about each argument and option, run `TwitchChatOffset transform-one-to-many --help`.

Read the above section on `transform-many-to-many`, as the same information is relevant here. The only difference is that the exhaustive list of all available options in the CSV file is defined [here](TwitchChatOffset/Csv/TransformOneToManyCsvNullables.cs), though they are the same bar the `input-file` column.
### Examples
Aim: apply all transformations to `chat.json` as listed in `transform-one-to-many.csv`, such that if for any transformation no `format` is specified, default it to plaintext.
```
TwitchChatOffset transform-one-to-many chat.json transform-one-to-many.csv -f plaintext
```
