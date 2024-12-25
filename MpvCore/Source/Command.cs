using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class Command
{
    private Process? _process = null;

    private const string PauseCommand = "kill -TSTP {0}";
    private const string ResumeCommand = "kill -CONT {0}";

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
            _process = Shell.StartProcess(Name);

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

    public async Task Pause()
    {
        if (!Paused && _process != null)
        {
            var pause = Shell.StartProcess(string.Format(PauseCommand, _process.Id));

            pause.Start();
            await pause.WaitForExitAsync();

            Paused = true;
        }
    }

    public async Task Resume()
    {
        if (Paused && _process != null)
        {
            var resume = Shell.StartProcess(string.Format(ResumeCommand, _process.Id));

            resume.Start();
            await resume.WaitForExitAsync();

            Paused = false;
        }
    }
}
