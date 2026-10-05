using System.Xml.Linq;

namespace ShellEmulator.Vfs;

public static class XmlVfsLoader
{
    public static VirtualFileSystem Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("VFS file not found", path);
        }

        try
        {
            var document = XDocument.Load(path);
            var element = document.Root
                ?? throw new FormatException("VFS XML has no root element");
            var root = Parse(element, "/");
            
            root.Name = "/";
            
            return new VirtualFileSystem(root) { Current = root };
        }
        catch (System.Xml.XmlException ex)
        {
            throw new FormatException("invalid VFS XML: " + ex.Message, ex);
        }
    }

    private static VfsNode Parse(XElement element, string path)
    {
        var isDirectory = string.Equals(
            (string?)element.Attribute("type"),
            "dir",
            StringComparison.OrdinalIgnoreCase);
        var content = isDirectory ? "" : ReadContent(element);
        var node = new VfsNode(
            (string?)element.Attribute("name") ?? path,
            isDirectory,
            content);

        node.Owner = (string?)element.Attribute("owner");
        
        foreach (var child in element.Elements("node"))
        {
            node.Children.Add(Parse(child, path + child.Attribute("name")?.Value));
        }
        
        return node;
    }

    private static string ReadContent(XElement element)
    {
        var encoding = (string?)element.Attribute("encoding");
        
        if (!string.Equals(encoding, "base64", StringComparison.OrdinalIgnoreCase))
        {
            return element.Value;
        }

        try
        {
            return System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(element.Value.Trim()));
        }
        catch (FormatException ex)
        {
            throw new FormatException("invalid base64 VFS content", ex);
        }
    }
}
