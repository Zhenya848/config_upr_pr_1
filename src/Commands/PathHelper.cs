using ShellEmulator.Vfs;

namespace ShellEmulator.Commands;

internal static class PathHelper
{
    public static VfsNode RequireNode(VirtualFileSystem vfs, string path)
    {
        return vfs.Resolve(path)
            ?? throw new FileNotFoundException("path not found", path);
    }

    public static VfsNode RequireDirectory(VirtualFileSystem vfs, string path)
    {
        var node = RequireNode(vfs, path);
        
        if (!node.IsDirectory)
        {
            throw new ArgumentException($"not a directory: {path}");
        }
        
        return node;
    }
}
