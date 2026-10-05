namespace ShellEmulator.Shell;

public static class CommandParser
{
    public static (string Command, string[] Args) Parse(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length == 0)
        {
            return ("", []);
        }
        
        return (parts[0].ToLowerInvariant(), parts[1..]);
    }
}
