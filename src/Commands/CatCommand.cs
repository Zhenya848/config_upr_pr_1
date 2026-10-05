using ShellEmulator.Shell;
using ShellEmulator.Vfs;

namespace ShellEmulator.Commands;

public sealed class CatCommand : ICommand
{
    public CommandResult Execute(VirtualFileSystem vfs, string[] args)
    {
        if (args.Length != 1) 
            throw new ArgumentException("cat requires exactly one argument");
        
        var file = PathHelper.RequireNode(vfs, args[0]);
        
        if (file.IsDirectory) 
            throw new ArgumentException("cat cannot read a directory");
        
        Console.Write(file.Content);
        
        if (!file.Content.EndsWith('\n')) 
            Console.WriteLine();
        
        return CommandResult.Ok();
    }
}
