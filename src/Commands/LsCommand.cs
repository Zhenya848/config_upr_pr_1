using ShellEmulator.Shell;
using ShellEmulator.Vfs;

namespace ShellEmulator.Commands;

public sealed class LsCommand : ICommand
{
    public CommandResult Execute(VirtualFileSystem vfs, string[] args)
    {
        if (args.Length > 1) 
            throw new ArgumentException("ls accepts at most one argument");
        
        var node = args.Length == 0 ? vfs.Current : PathHelper.RequireNode(vfs, args[0]);
        
        if (!node.IsDirectory)
        {
            Console.WriteLine(node.Name);
            return CommandResult.Ok();
        }

        foreach (var child in node.Children.OrderBy(x => x.Name, StringComparer.Ordinal))
        {
            Console.WriteLine(child.Name + (child.IsDirectory ? "/" : ""));
        }
        
        return CommandResult.Ok();
    }
}
