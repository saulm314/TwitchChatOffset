namespace TwitchChatOffset;

public static class OffsetHandler
{
    // end=-1 is to be treated as no ending provided, therefore we keep everything until the end of the VOD
    public static void Handle(string input, string output, long start, long end)
    {
        System.Console.WriteLine($"Handling offset with parameters {input} {output} {start} {end}");
    }
}