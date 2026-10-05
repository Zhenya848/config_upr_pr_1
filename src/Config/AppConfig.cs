using System.Text;

namespace ShellEmulator.Config;

public sealed record AppConfig(
    string VfsPath,
    string? ScriptPath,
    string? ConfigPath,
    bool ConfigWasLoaded)
{
    public static AppConfig FromArguments(string[] args)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        for (var i = 0; i < args.Length; i++)
        {
            if (!args[i].StartsWith("--", StringComparison.Ordinal))
            {
                throw new ArgumentException($"unknown argument: {args[i]}");
            }

            var key = args[i][2..];
            
            if (key is "help" or "h")
            {
                PrintHelp();
                Environment.Exit(0);
            }

            if (i + 1 >= args.Length)
            {
                throw new ArgumentException($"missing value for --{key}");
            }

            values[key] = args[++i];
        }

        values.TryGetValue("vfs", out var vfs);
        values.TryGetValue("script", out var script);
        values.TryGetValue("config", out var config);
        
        return new AppConfig(vfs ?? "vfs_examples/sample.xml", script, config, false);
    }

    public static AppConfig Load(AppConfig commandLine)
    {
        if (string.IsNullOrWhiteSpace(commandLine.ConfigPath))
        {
            return commandLine with { VfsPath = Path.GetFullPath(commandLine.VfsPath) };
        }

        var configPath = Path.GetFullPath(commandLine.ConfigPath);
        var fileValues = TomlSubset.Load(configPath);
        var vfs = fileValues.TryGetValue("vfs_path", out var fileVfs)
            ? fileVfs
            : commandLine.VfsPath;
        var script = fileValues.TryGetValue("script_path", out var fileScript)
            ? fileScript
            : commandLine.ScriptPath;
        
        return new AppConfig(
            Path.GetFullPath(vfs),
            string.IsNullOrWhiteSpace(script) ? null : Path.GetFullPath(script),
            configPath,
            true);
    }

    public void PrintDebug()
    {
        Console.WriteLine("[config]");
        Console.WriteLine($"vfs_path = {VfsPath}");
        Console.WriteLine($"script_path = {ScriptPath ?? "<none>"}");
        Console.WriteLine($"config_path = {ConfigPath ?? "<none>"}");
        Console.WriteLine($"config_loaded = {ConfigWasLoaded}");
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Shell emulator, variant 4");
        Console.WriteLine("Usage: dotnet run -- [--vfs PATH] [--script PATH] [--config PATH]");
    }
}

internal static class TomlSubset
{
    public static Dictionary<string, string> Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("configuration file not found", path);
        }

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var raw in File.ReadAllLines(path, Encoding.UTF8))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            var separator = line.IndexOf('=');
            
            if (separator <= 0)
            {
                throw new FormatException("invalid TOML line: " + raw);
            }

            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim();
            
            result[key] = ParseString(value);
        }

        return result;
    }

    private static string ParseString(string value)
    {
        if (value.Length < 2 || value[0] != '"' || value[^1] != '"')
        {
            throw new FormatException("only double-quoted TOML strings are supported");
        }

        return value[1..^1].Replace("\\\"", "\"").Replace("\\\\", "\\");
    }
}
