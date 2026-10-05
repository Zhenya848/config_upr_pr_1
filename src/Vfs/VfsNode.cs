namespace ShellEmulator.Vfs;

public sealed class VfsNode
{
    public VfsNode(string name, bool isDirectory, string content = "")
    {
        Name = name;
        IsDirectory = isDirectory;
        Content = content;
    }

    public string Name { get; set; }
    public bool IsDirectory { get; }
    public string Content { get; set; }
    public string? Owner { get; set; }
    public List<VfsNode> Children { get; } = [];

    public VfsNode? Find(string name)
    {
        return Children.FirstOrDefault(child =>
            string.Equals(child.Name, name, StringComparison.Ordinal));
    }
}
