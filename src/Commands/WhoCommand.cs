using ShellEmulator.Shell;
using ShellEmulator.Vfs;

namespace ShellEmulator.Commands;

public sealed class WhoCommand : ICommand
{
    public CommandResult Execute(VirtualFileSystem vfs, string[] args)
    {
        if (args.Length != 0) 
            throw new ArgumentException("who accepts no arguments");
        
        Console.WriteLine(Environment.UserName);
        
        return CommandResult.Ok();
    }
}
