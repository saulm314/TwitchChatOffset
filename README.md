# TwitchChatOffset
This is TwitchChatOffset.

The purpose of this software is to take a JSON Twitch chat file, apply various transformations to it, and serialize it back into one of various formats (JSON, YTT, ASS, plaintext). The primary uses are for cropping/delaying Twitch chats and for converting JSON Twitch chat files into YTT YouTube subtitle files (or general ASS subtitle files that can be played in a local video player such as VLC), so that the Twitch chat can be replayed directly in YouTube, as seen [here](https://youtu.be/y7_ZrMJNHUk).

For all transformations, TwitchChatOffset supports:
- configuring the start time (and setting it as the new zero point) and dismissing all messages before that point
- configuring the end time and dismissing all messages after that point, or allowing messages without any end limit
- configuring a delay to apply to all messages after they have been trimmed (for Twitch highlights/VODs/clips, a delay of 5 seconds is recommended to account for streaming delay)
- choosing which format to convert the Twitch chat to (JSON, JSON indented, YTT, ASS, plaintext)

When converting to YTT/ASS subtitles, TwitchChatOffset supports:
- preserving the original username colours
- generating random colours for usernames without a colour (but keeping the same colour for each user in the same file)
- choosing which corner (or midpoint) the subtitles appear in
- configuring the background window opacity (fully transparent to fully opaque)
- configuring the maximum number of messages that appear on the screen at a time
- configuring the maximum number of characters before the text wraps to the next line
- tailoring other formatting options, such as text colour (apart from username), text size, text shadow, text shadow colour

TwitchChatOffset also supports bulk transformations, so if you have many Twitch chats (or one big Twitch chat that needs to split into several videos), you can put the details of each chat into a CSV table and TwitchChatOffset will automatically apply all the transformations into as many files as needed. TwitchChatOffset also supports easily making several transformations per input file (e.g. converting the same Twitch chat JSON file into both YTT and ASS formats), and performs optimisations to avoid doing duplicate work for the same input file.

To handle the YTT conversions, the app uses a fork of [YTSubConverter](https://github.com/arcusmaximus/YTSubConverter).

TwitchChatOffset can be run on any operating system that supports the .NET Runtime (Windows, MacOS, Linux), though you may need to install .NET manually if you don't have it already.

TwitchChatOffset is available as a console application only.

## WARNING
**TwitchChatOffset will only warn you about overwriting files if for any given transformation the input file is the same as the output file. If the output file differs from the input file, or if the output file is the same as an input file from a different transformation, no warning will be given!**
# Installation
To install the app, first download the latest release [here](https://github.com/saulm314/TwitchChatOffset/releases), and extract the ZIP file.

Then, to run the app, open a terminal and navigate to the folder where you extracted the ZIP file, and run `.\TwitchChatOffset.exe` on Windows or `dotnet TwitchChatOffset.dll` on any operating system. If it doesn't work, you likely don't have the .NET runtime installed. The required .NET version is 10+, so you can download it [here](https://dotnet.microsoft.com/en-us/download/dotnet/10.0/runtime) and once you have it installed try running the app again.

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
TwitchChatOffset transform chat.json transformed-chat.ytt --start 3600 --end 10800 --delay 60 -f ytt --sub-position topright
```
## Transform Many
Using a CSV file with a list of input JSON files and transformations, apply all the listed transformations and create an output file for each, or multiple output files for each. For the CSV file, use the template provided [here](templates/transform-many.csv).
```
TwitchChatOffset transform-many <csv-path> [options]
```
To see more information about each argument and option, run `TwitchChatOffset transform-many --help`.

Most of the options that are available on the command line are also available in the CSV file with the same alias but without leading hyphens. For example, the `--sub-position` command-line option is also available as a CSV option as `sub-position`. An exhaustive list of all available options in the CSV file is defined [here](TwitchChatOffset/Options/Groups/TransformManyCommonOptions.cs). Options may be specified in any order.

The same option can be specified both in the CSV file and on the command line. In that case, the program uses the value from the CSV file and ignores the one from the command line. A field or an entire column can also be left blank in the CSV file. In that case, for that file, the value provided on the command line is used instead. If no value is provided on the command line, then the hardcoded default value is used (run the `--help` option to see default values).

The CSV file can contain redundant columns and the program will ignore them. Though be careful, because some apps such as Google Sheets allow you to put a newline `\n` character in a field, which is not allowed in CSV and will lead your CSV file to be parsed incorrectly. The CSV file can also contain redundant rows but the program will complain about it (though it will handle it gracefully - you will simply see a message that that row is being skipped).

You can specify the same input file in multiple rows of the CSV document, and the program will perform multiple transformations accordingly. Don't worry if rows with identical input files are not adjacent - the program will automatically sort the rows and carry out the transformations in an optimised manner (e.g. by reusing computations from a previous transformation with the same input file).

If you need to perform multiple transformations for each input file (e.g. one for JSON format, one for YTT format, one for ASS format, one for TXT format), the CSV document can get a bit cluttered if you have to repeat all those rows in their entirety while just changing a handful of values. To make it easier to manage your data, you may specify multiple transformations on a single row. To do this, specify all of the options that you need for one transformation, then append a `/next` column and add options for another transformation to the right of that column (and optionally keep adding as many `/next` columns to produce as many transformations per input file as you please). After the `/next` column, you do not need to repeat all the options that come before the `/next` column: you only need to repeat the ones that are different. In a document with `/next` columns, the program parses each row as follows:
- First, all data up to the first `/next` column is collected and will result in a single transformation (and a single output file)
  - If any fields up to this point are left blank, then the command line value is used instead, or the hardcoded default value if no value is specified on the command line either
- Then, the program makes a copy of all the data up to that point, and reuses it after the `/next` column
- Any non-empty fields after the `/next` column overwrite the existing data
- Then after the next `/next` column after that, a copy of the previous data is made again, and so on...

While options are allowed to be repeated if there is a `/next` column in between, with a single group surrounded by the start/end of the file and/or `/next` columns, repetitions are not allowed and will throw an error.

It is best to understand this with an example, so see the template provided above for more clarity.
### Examples
Aim: to transform all files in `transform-many.csv`, such that if for any file no `sub-position` is specified, default it to bottom-right.
```
TwitchChatOffset transform-many transform-many.csv --sub-position bottomright
```
## Transform All
Find all files that have a name with a matching search pattern in a given directory, then apply a transformation or multiple transformations to each of them, and create a new output file (or multiple output files) for each input file.
```
TwitchChatOffset transform-all [options]
```
To see more information about each argument and option, run `TwitchChatOffset transform-all --help`.

Use this command similar to the `transform` command, but if you need to apply the same transformation or set of transformations to all the files in a directory. Use the `-i` option to select the input directory, and the `-o` option to select your output directory. You can also use the `--search-pattern` option to specify which files in the input directory to transform, e.g. `*.json` for all JSON files, or `*chat*.json` for all JSON files with "chat" in their names. Use `*` to represent zero or more characters, or `?` to represent exactly one character, as is documented on [this](https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles?view=net-10.0) page. You may also find the `--suffix` option particularly useful: by default all output files will keep the same names as their input files, but you can specify a suffix to remove the file's extension and append a new suffix onto it, e.g. `.ytt` or `-transformed.ytt`.

Note that merely running `TwitchChatOffset transform-all` without specifying your input or output directories (`-i`, `-o`) finds all JSON files in the current directory, performs a transformation for each, and then replaces the same file. This is of course dangerous, however if you do accidentally leave the input and output directories the same, the program will give you a warning and ask if you want to proceed. **The program will not give you a warning if the input and output directories are different, even if there are already files that will be overwritten in the output directory!**

If you wish to specify multiple transformations per input file in a `transform-all` command, you can do this with a CSV file, just like we did with the `transform-many` command. The only difference is that on this CSV file the `input-file` and `output-file` options are not recognised (see template [here](templates/transform-all.csv) and an exhaustive list of all available CSV options [here](TwitchChatOffset/Options/Groups/TransformAllCommonOptions.cs)). Every transformation will simply apply to every input file and output files will be generated based on the given output directory and suffix. You can either specify multiple transformations across multiple rows, or use the `/next` technique from the `transform-many` command to specify multiple transformations within the same row. If you wish to do this, simply add a `--csv` option and specify the path to your CSV file.
### Examples
Aim: to find all JSON files in the current directory, cut out the first minute of their chats, and store the resulting chats in new JSON files in the `transformed` directory, with each output file having the same name as its corresponding input file.
```
TwitchChatOffset transform-all -o transformed --start 60
```
Aim: as before but this time we want to leave the output files in the same directory, but adding `-transformed` to their names to differentiate them from the input files. E.g. `chat.json` becomes `chat-transformed.json`.
```
TwitchChatOffset transform-all --suffix "-transformed.json" --start 60
```
Aim: same as the previous example, but this time we only want to apply transformations to JSON files that have "`chat`" in their names.
```
TwitchChatOffset transform-all --suffix "-transformed.json" --start 60 --pattern "*chat*.json"
```
Aim: same as the previous example, but this time we want to perform several transformations that we will specify in `transform-all.csv` (along with suffixes and formats).
```
TwitchChatOffset transform-all --start 60 --pattern "*chat*.json" --csv transform-all.csv
```
