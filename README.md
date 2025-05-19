This is TwitchChatOffset.

The purpose of this software is to take a JSON Twitch chat file, delete all messages up to a particular offset, and off-set all remaining messages by that amount. Also, (optionally) all messages after a desired end point can be deleted.

For example, if you had a JSON Twitch chat file from a VOD that was 3.5 hours long, but you only wanted the chat from the 1-3 hour marks, then this tool will help you take out all the messages from the first hour and all the messages from the last half hour, and then set the 1 hour mark as the "zero" mark for the remaining messages. For example, if there was a message at the 1:00:31 mark, then TwitchChatOffset will convert the time of that message to 0:00:31.

This tool also provides some other features, such as formatting the output JSON file into indented form or converting the Twitch chat into a human-readable text file. More similar features may be added in the future as these are quite trivial to add.

This tool works on Windows only. It may work on other operating systems too, but will likely require installing external software. Look up how to run a .NET executable on your operating system.

# Usage

To use this software, download the latest release and unzip it. From there, in my opinion the easiest way to use it is to take the path of the folder where you unzipped it, and add that path to your PATH environment variable. (Windows key -> "environment variables" -> Edit the system environment variables -> Environment Variables... -> Path (double click) -> New -> [paste your path])

Otherwise, you will have to specify the full path of where you extracted the `TwitchChatOffset.exe` file every time you use the software.

Then, open PowerShell and navigate to the directory with your Twitch chat json file (e.g. `cd D:\TwitchDownloader`). Then simply run `TwitchChatOffset` with the `transform` command, specifying the input file, the output file, the optional start offset in seconds, and optionally the ending time in seconds. For example, if the input file is `twitchChat.json`, the desired start offset is exactly the 1 hour mark, and the desired end point is exactly the 3 hour mark, then you would run:

`TwitchChatOffset transform twitchChat.json twitchChatNew.json --start 3600 --end 10800 -f JsonIndented`

(The `-f`/`--format` option is optional but makes it more human-readable.) Or, if you didn't add the software to your PATH variable, then you have to specify the exact path of the `TwitchChatOffset.exe` file, e.g.:

`D:\TwitchChatOffset\TwitchChatOffset.exe transform twitchChat.json twitchChatNew.json --start 3600 --end 10800 -f JsonIndented`

To convert the Twitch chat file to human-readable plain-text form, simply run:

`TwitchChatOffset transform twitchChat.json twitchChatNew.txt -f Plaintext`

again optionally specifying `--start` and `--end` options.

# Bulk Transformations

TwitchChatOffset also supports performing many transformations in one go. In this case, the transformation data cannot be passed over the command line, and is instead read from a CSV file. Find template CSV files [here](templates).

## `transform-many-to-many`

If you have many JSON chat files and would like to trim each, then use the `transform-many-to-many` command, specify your CSV file with the transformation data, optionally specify an output directory, and optionally specify formatting (e.g. `JsonIndented`). E.g.:

`TwitchChatOffset transform-many-to-many data.csv -o trimmed-chats -f JsonIndented`

Note: with this command, each input file found in the CSV table will be opened, and if you repeat the same input file many times, that file will be reopened and reparsed many times. This is very inefficient, so if you need to extract multiple outputs from a single JSON file, use the `transform-one-to-many` command as shown below.

## `transform-one-to-many`

If you have one JSON chat file and would like to split it up into many, then use the `transform-one-to-many` command, specify the input JSON file, the CSV file, optionally an output directory, and optionally a formatting option. E.g.:

`TwitchChatOffset transform-one-to-many chat.json data.csv -o trimmed-chats -f JsonIndented`

## `transform-all`

If you want to transform all JSON files in a directory by the same parameters (i.e. same format, same start and end times if applicable, etc.), use the `transform-all` command, specify a suffix for the output files (including the extension), and optionally specify any options. E.g.:

`TwitchChatOffset transform-all ".txt" -i input-directory --search-pattern "0*.json" -o output-directory -f Plaintext`

This will find all files in `input-directory` whose names start with `0` and have a `.json` extension, convert each file into plaintext, and write it to `output-directory` with the same name as the original file, but the `.json` extension replaced with a `.txt` extension.

## All Bulk Transformations

For any bulk transformation, if you prefer the application to not print to the console every time it finishes writing a file, pass the `-q`/`--quiet` option.

**WARNING: If any files with the same name already exist, they will automatically be overwritten without warning! To avoid losing files, double check that the output file names are correct, or make copies before starting a transformation!**

# YouTube Captions

Although this could also be used for other reasons too, the original reason this script was written was to parse messages from a long Twitch VOD into a usable format so that they can be converted to YouTube captions for a video that is only a small part of the Twitch VOD as a whole.

To do this, you can take the following steps:
- Use [Twitch Downloader](https://github.com/lay295/TwitchDownloader) to download the JSON of the Twitch Chat:

`TwitchDownloaderCLI ChatDownload -u videoID -o videoID.json`

- Use TwitchChatOffset to trim the chat according to your needs:

`TwitchChatOffset transform videoID.json videoIDNew.json --start 3600 --end 10800 --formatting JsonIndented`

- Run [this](https://gist.github.com/Cqoicebordel/d9110b4b1191b9e9f6a8165438e00ea0) Python script to convert the JSON file into a YTT file and upload that file as a YouTube subtitle
