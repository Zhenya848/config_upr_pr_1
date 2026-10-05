using ShellEmulator.Config;
using ShellEmulator.Shell;
using ShellEmulator.Vfs;

namespace ShellEmulator;

public static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            var raw = AppConfig.FromArguments(args);
            var config = AppConfig.Load(raw);
            
            config.PrintDebug();
            
            var vfs = XmlVfsLoader.Load(config.VfsPath);
            var shell = new ShellSession(vfs);
            
            return config.ScriptPath is null
                ? shell.RunInteractive()
                : shell.RunScript(config.ScriptPath);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"error: {ex.Message}");
            return 1;
        }
    }
}
