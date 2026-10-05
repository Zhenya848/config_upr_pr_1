namespace ShellEmulator.Vfs;

public sealed class VirtualFileSystem
{
    public VirtualFileSystem(VfsNode root)
    {
        Root = root;
        Current = root;
    }

    public VfsNode Root { get; }
    public VfsNode Current { get; set; }

    public VfsNode? Resolve(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || path == ".")
        {
            return Current;
        }

        var node = path.StartsWith('/') ? Root : Current;
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var part in parts)
        {
            if (part == ".") 
                continue;
            
            if (part == "..")
            {
                node = FindParent(Root, node) ?? Root;
                continue;
            }

            node = node.Find(part)
                ?? throw new FileNotFoundException("path not found", path);
        }

        return node;
    }

    public string GetPath(VfsNode target)
    {
        var parts = new Stack<string>();
        var current = target;
        
        while (current != Root)
        {
            parts.Push(current.Name);
            current = FindParent(Root, current)
                ?? throw new InvalidOperationException("VFS tree is inconsistent");
        }
        
        return "/" + string.Join('/', parts);
    }

    public bool RemoveChild(VfsNode parent, string name)
    {
        var node = parent.Find(name);
        
        return node is not null && parent.Children.Remove(node);
    }

    private static VfsNode? FindParent(VfsNode root, VfsNode target)
    {
        foreach (var child in root.Children)
        {
            if (child == target) 
                return root;
            
            var result = FindParent(child, target);
            
            if (result is not null) 
                return result;
        }
        
        return null;
    }
}
