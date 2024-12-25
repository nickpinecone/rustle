using System.Diagnostics;
using System.Threading.Tasks;

public static class Shell
{
    private const string PauseCommand = "kill -TSTP {0}";
    private const string ResumeCommand = "kill -CONT {0}";

    public static string BashPath { get; set; } = "/bin/bash";

    public static Process Create(string command)
    {
        return new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = BashPath,
                Arguments = $"-c \"{command}\"",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
    }

    public static void Start(Process process)
    {
        process.Start();
    }

    public static void Stop(Process process)
    {
        process.Kill();
        process.Dispose();
    }

    public static async Task Pause(Process process)
    {
        var pause = Shell.Create(string.Format(PauseCommand, process.Id));
        pause.Start();
        await pause.WaitForExitAsync();
    }

    public static async Task Resume(Process process)
    {
        var resume = Shell.Create(string.Format(ResumeCommand, process.Id));
        resume.Start();
        await resume.WaitForExitAsync();
    }
}
