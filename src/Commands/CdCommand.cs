using ShellEmulator.Shell;
using ShellEmulator.Vfs;

namespace ShellEmulator.Commands;

public sealed class CdCommand : ICommand
{
    public CommandResult Execute(VirtualFileSystem vfs, string[] args)
    {
        if (args.Length != 1) 
            throw new ArgumentException("cd requires exactly one argument");
        
        var target = PathHelper.RequireDirectory(vfs, args[0]);
        vfs.Current = target;
        
        return CommandResult.Ok();
    }
}
