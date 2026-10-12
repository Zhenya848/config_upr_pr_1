using ShellEmulator.Shell;
using ShellEmulator.Vfs;

namespace ShellEmulator.Commands;

public sealed class CatCommand : ICommand
{
    public CommandResult Execute(
        VirtualFileSystem vfs,
        string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException(
                "cat: missing operand");
        }

        var hasErrors = false;

        foreach (var path in args)
        {
            try
            {
                var node = PathHelper.RequireNode(vfs, path);

                if (node.IsDirectory)
                {
                    Console.Error.WriteLine(
                        $"cat: {path}: Is a directory");

                    hasErrors = true;
                    continue;
                }

                node.AccessTime = DateTimeOffset.UtcNow;

                Console.Write(node.Content);
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine(
                    $"cat: {path}: No such file or directory");

                hasErrors = true;
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(
                    $"cat: {path}: {ex.Message}");

                hasErrors = true;
            }
        }

        return hasErrors
            ? CommandResult.Fail()
            : CommandResult.Ok();
    }
}