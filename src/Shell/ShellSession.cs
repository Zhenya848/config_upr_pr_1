using System.Runtime.InteropServices;
using ShellEmulator.Commands;
using ShellEmulator.Vfs;

namespace ShellEmulator.Shell;

public sealed class ShellSession
{
    private readonly VirtualFileSystem _vfs;
    private readonly Dictionary<string, ICommand> _commands;

    public ShellSession(VirtualFileSystem vfs)
    {
        _vfs = vfs;
        _commands = new Dictionary<string, ICommand>(StringComparer.Ordinal)
        {
            ["ls"] = new LsCommand(),
            ["cd"] = new CdCommand(),
            ["cat"] = new CatCommand(),
            ["who"] = new WhoCommand(),
            ["head"] = new HeadCommand(),
            ["touch"] = new TouchCommand(),
        };
    }

    public int RunInteractive()
    {
        while (true)
        {
            Console.Write(Prompt());
            var line = Console.ReadLine();
            if (line is null) return 0;
            var result = Execute(line);
            if (result.ExitRequested) return 0;
        }
    }

    public int RunScript(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("start script not found", path);
        }

        foreach (var raw in File.ReadLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            Console.WriteLine(Prompt() + line);
            var result = Execute(line);
            if (result.ExitRequested) break;
        }
        return 0;
    }

    public CommandResult Execute(string input)
    {
        var (name, args) = CommandParser.Parse(input);
        
        if (name == "exit") 
            return CommandResult.Exit();
        
        if (!_commands.TryGetValue(name, out var command))
        {
            Console.WriteLine($"error: unknown command '{name}'");
            
            return CommandResult.Fail();
        }

        try
        {
            return command.Execute(_vfs, args);
        }
        catch (Exception ex) when (ex is ArgumentException or FileNotFoundException)
        {
            Console.WriteLine("error: " + ex.Message);
            return CommandResult.Fail();
        }
    }

    private string Prompt()
    {
        var user = Environment.UserName;
        var host = Environment.MachineName;
        var separator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\\" : "/";
        var path = _vfs.GetPath(_vfs.Current).Replace('/', separator[0]);
        
        return $"{user}@{host}:{path}$ ";
    }
}

public interface ICommand
{
    CommandResult Execute(VirtualFileSystem vfs, string[] args);
}
