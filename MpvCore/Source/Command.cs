using System;
using System.Diagnostics;

public class Command
{
    private Process? _process = null;

    private const string PauseCommand = "kill -TSTP {0}";
    private const string ResumeCommand = "kill -CONT {0}";

    public string BashPath { get; set; } = "/bin/bash";

    public string Name { get; private set; }
    public bool Paused { get; private set; }

    public Command(string name)
    {
        Name = name.Replace("\"", "\\\"");
    }

    public void Start()
    {
        if (_process == null)
        {
            _process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = BashPath,
                    Arguments = $"-c \"{Name}\"",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            _process.Exited += HandleFinished;
            _process.Disposed += HandleFinished;
            _process.ErrorDataReceived += HandleFinished;

            _process.Start();
        }
    }

    private void HandleFinished(object? sender, EventArgs e)
    {
        _process = null;
        Paused = false;
    }

    public void Stop()
    {
        if (_process != null)
        {
            _process.Kill();
            _process.Dispose();
        }
    }

    public async void Pause()
    {
        if (!Paused && _process != null)
        {
            var pause = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = BashPath,
                    Arguments = $"-c \"{string.Format(PauseCommand, _process.Id)}\"",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            pause.Start();
            await pause.WaitForExitAsync();

            Paused = true;
        }
    }

    public async void Resume()
    {
        if (Paused && _process != null)
        {
            var resume = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = BashPath,
                    Arguments = $"-c \"{string.Format(ResumeCommand, _process.Id)}\"",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            resume.Start();
            await resume.WaitForExitAsync();

            Paused = false;
        }
    }
}
