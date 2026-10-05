using ShellEmulator.Shell;
using ShellEmulator.Vfs;

namespace ShellEmulator.Commands;

public sealed class HeadCommand : ICommand
{
    private const int DefaultLines = 10;

    public CommandResult Execute(VirtualFileSystem vfs, string[] args)
    {
        var (lineCount, path) = ParseArguments(args);
        var file = PathHelper.RequireNode(vfs, path);
        
        if (file.IsDirectory) 
            throw new ArgumentException("head cannot read a directory");
        
        var lines = file.Content.Replace("\r", "").Split('\n');
        var limit = Math.Min(lineCount, lines.Length);
        
        for (var i = 0; i < limit; i++) 
            Console.WriteLine(lines[i]);
        
        return CommandResult.Ok();
    }

    private static (int Count, string Path) ParseArguments(string[] args)
    {
        if (args.Length == 1) 
            return (DefaultLines, args[0]);
        
        if (args.Length == 3 && args[0] == "-n" && int.TryParse(args[1], out var count) && count >= 0)
        {
            return (count, args[2]);
        }
        
        throw new ArgumentException("usage: head [-n NUMBER] PATH");
    }
}
