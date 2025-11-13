using System.IO;

namespace TwitchChatOffset.CommandLine;

public static class IOUtils
{
    extension(File)
    {
        public static bool ValidateFileExists(string inputPath)
        {
            if (File.Exists(inputPath))
                return true;
            PrintError($"Input file {inputPath} could not be found", 2);
            return false;
        }
    }
}