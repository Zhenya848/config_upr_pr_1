using ShellEmulator.Shell;
using ShellEmulator.Vfs;

namespace ShellEmulator.Commands;

public sealed class TouchCommand : ICommand
{
    public CommandResult Execute(VirtualFileSystem vfs, string[] args)
    {
        if (args.Length != 1) 
            throw new ArgumentException("touch requires exactly one path");
        
        var path = args[0];
        
        if (path == "/" || path.EndsWith('/')) 
            throw new ArgumentException("invalid file path");

        var separator = path.LastIndexOf('/');
        var parentPath = separator == -1 ? "." : path[..separator];
        var name = path[(separator + 1)..];
        var parent = PathHelper.RequireDirectory(vfs, parentPath);
        var existing = parent.Find(name);
        
        if (existing is not null)
        {
            if (existing.IsDirectory) 
                throw new ArgumentException("path is a directory");
            
            return CommandResult.Ok();
        }
        
        parent.Children.Add(new VfsNode(name, false, ""));
        
        return CommandResult.Ok();
    }
}
