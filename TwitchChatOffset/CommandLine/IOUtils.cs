using System.IO;

namespace TwitchChatOffset.CommandLine;

public static class IOUtils
{
    public static bool ValidateInputFileNameNotEmpty(string inputFile)
    {
        if (!string.IsNullOrEmpty(inputFile))
            return true;
        PrintError("Input file name must not be empty; skipping...", 1);
        return false;
    }

    public static bool ValidateOutputFileNameNotEmpty(string outputFile)
    {
        if (!string.IsNullOrEmpty(outputFile))
            return true;
        PrintError("Input file name must not be empty; skipping...", 1);
        return false;
    }

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