using ShellEmulator.Shell;
using ShellEmulator.Vfs;

namespace ShellEmulator.Commands;

public sealed class TouchCommand : ICommand
{
    public CommandResult Execute(
        VirtualFileSystem vfs,
        string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException(
                "touch: missing file operand");
        }

        var hasErrors = false;

        foreach (var path in args)
        {
            try
            {
                TouchPath(vfs, path);
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(
                    $"touch: {path}: {ex.Message}");

                hasErrors = true;
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine(
                    $"touch: {path}: No such file or directory");

                hasErrors = true;
            }
        }

        return hasErrors
            ? CommandResult.Fail()
            : CommandResult.Ok();
    }

    private static void TouchPath(
        VirtualFileSystem vfs,
        string path)
    {
        if (path == "/" || path.EndsWith('/'))
        {
            throw new ArgumentException(
                "invalid file path");
        }

        var separator = path.LastIndexOf('/');

        var parentPath =
            separator == -1
                ? "."
                : path[..separator];

        var name = path[(separator + 1)..];

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "invalid file name");
        }

        var parent =
            PathHelper.RequireDirectory(
                vfs,
                parentPath);

        var existing = parent.Find(name);

        var now = DateTimeOffset.UtcNow;

        if (existing is not null)
        {
            if (existing.IsDirectory)
            {
                throw new ArgumentException(
                    "Is a directory");
            }

            existing.AccessTime = now;
            existing.ModificationTime = now;
            existing.ChangeTime = now;

            Console.WriteLine(
                $"updated: {path}");

            Console.WriteLine(
                $"mtime: {FormatTime(existing.ModificationTime)}");

            return;
        }

        var node = new VfsNode(
            name,
            false,
            "");

        node.AccessTime = now;
        node.ModificationTime = now;
        node.ChangeTime = now;

        parent.Children.Add(node);

        Console.WriteLine(
            $"created: {path}");

        Console.WriteLine(
            $"mtime: {FormatTime(node.ModificationTime)}");
    }

    private static string FormatTime(
        DateTimeOffset timestamp)
    {
        return timestamp
            .ToLocalTime()
            .ToString(
                "yyyy-MM-dd HH:mm:ss zzz");
    }
}