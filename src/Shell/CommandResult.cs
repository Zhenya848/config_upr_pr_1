namespace ShellEmulator.Shell;

public readonly record struct CommandResult(bool Success, bool ExitRequested)
{
    public static CommandResult Ok() => new(true, false);
    public static CommandResult Fail() => new(false, false);
    public static CommandResult Exit() => new(true, true);
}
