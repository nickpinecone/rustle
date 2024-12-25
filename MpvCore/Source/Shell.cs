using System.Diagnostics;

public static class Shell
{
    public static string BashPath { get; set; } = "/bin/bash";

    public static Process StartProcess(string command)
    {
        return new Process() { StartInfo = new ProcessStartInfo() {
            FileName = BashPath,
            Arguments = $"-c \"{command}\"",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        } };
    }
}
