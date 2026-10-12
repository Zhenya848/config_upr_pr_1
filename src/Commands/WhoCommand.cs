using System.Diagnostics;
using ShellEmulator.Shell;
using ShellEmulator.Vfs;

namespace ShellEmulator.Commands;

public sealed class WhoCommand : ICommand
{
    public CommandResult Execute(
        VirtualFileSystem vfs,
        string[] args)
    {
        if (args.Length != 0)
        {
            throw new ArgumentException(
                "who: extra operand");
        }

        var user = Environment.UserName;
        var host = Environment.MachineName;
        var terminal = GetTerminalName();

        var loginTime =
            Process.GetCurrentProcess().StartTime;

        Console.WriteLine(
            "USER       TTY       LOGIN TIME          HOST");

        Console.WriteLine(
            $"{user,-10} " +
            $"{terminal,-9} " +
            $"{loginTime:MMM dd HH:mm}     " +
            $"{host}");

        return CommandResult.Ok();
    }

    private static string GetTerminalName()
    {
        if (OperatingSystem.IsWindows())
        {
            return "console";
        }

        var tty =
            Environment.GetEnvironmentVariable(
                "TTY");

        return string.IsNullOrWhiteSpace(tty)
            ? "tty"
            : Path.GetFileName(tty);
    }
}