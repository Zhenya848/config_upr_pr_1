using ShellEmulator.Shell;
using ShellEmulator.Vfs;

var root = new VfsNode("/", true);
var home = new VfsNode("home", true);
var hello = new VfsNode("hello.txt", false, "one\ntwo\nthree\n");
home.Children.Add(hello);
root.Children.Add(home);
var vfs = new VirtualFileSystem(root) { Current = root };
var shell = new ShellSession(vfs);

Test("ls", () => shell.Execute("ls").Success);
Test("cd", () => shell.Execute("cd home").Success && vfs.Current.Name == "home");
Test("cat", () => shell.Execute("cat hello.txt").Success);
Test("who", () => shell.Execute("who").Success);
Test("head", () => shell.Execute("head -n 2 hello.txt").Success);
Test("touch", () => shell.Execute("touch new.txt").Success && vfs.Current.Find("new.txt") is not null);
Test("unknown command", () => !shell.Execute("unknown").Success);
Test("exit", () => shell.Execute("exit").ExitRequested);

Console.WriteLine("All tests passed.");

static void Test(string name, Func<bool> check)
{
    if (!check()) throw new Exception("Test failed: " + name);
    Console.WriteLine("PASS: " + name);
}
