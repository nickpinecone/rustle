using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FFAudio;

internal static class Shell
{
    public static Process Create(string program, string args = "")
    {
        return new Process() { StartInfo = new ProcessStartInfo() {
            FileName = program,
            Arguments = args,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        } };
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
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var pause = Shell.Create("kill", $"-TSTP {process.Id}");
            pause.Start();
            await pause.WaitForExitAsync();
        }
    }

    public static async Task Resume(Process process)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var resume = Shell.Create("kill", $"-CONT {process.Id}");
            resume.Start();
            await resume.WaitForExitAsync();
        }
    }
}
