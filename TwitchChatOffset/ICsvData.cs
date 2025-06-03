namespace TwitchChatOffset;

public interface ICsvData
{
    string InputFile { get; }
    string OutputFile { get; }
    CsvOptions PCsvOptions { get; }

    // the consumer must call this method before attempting to access PCsvOptions,
    //      else an internal error will be thrown
    void WriteDefaultToEmptyFields(CsvOptions defaultOptions);
    protected bool DefaultOptionsWritten { get; }

    // if required data is not null, returns true; else returns false and gives an explanation
    // the consumer must call this method before attempting to access InputFile or OutputFile,
    //      else an internal error will be thrown
    bool NullablesGood(out string explanation);
    protected bool NullablesChecked { get; }

    void Deconstruct(out string inputFile, out string outputFile, out long start, out long end, out Format format, out string outputDir)
    {
        inputFile = InputFile;
        outputFile = OutputFile;
        (start, end, format, outputDir) = PCsvOptions;
    }
}