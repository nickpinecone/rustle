using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class Player
{
    private Process? _process = null;

    public bool Playing { get; private set; }
    public bool Paused { get; private set; }

    private void HandleFinished(object? sender, EventArgs e)
    {
        _process = null;

        Playing = false;
        Paused = false;
    }

    public async Task Play(string path)
    {
        if (Paused && _process != null)
        {
            await Shell.Resume(_process);

            Paused = false;
        }

        if (!Playing && _process == null)
        {
            _process = Shell.Create($"ffplay -nodisp -loglevel error -autoexit {path}");

            _process.Exited += HandleFinished;
            _process.Disposed += HandleFinished;
            _process.ErrorDataReceived += HandleFinished;

            Shell.Start(_process);

            Playing = true;
        }
    }

    public async Task Wait()
    {
        if (Playing && !Paused && _process != null)
        {
            await _process.WaitForExitAsync();
        }
    }

    public void Stop()
    {
        if (Playing && _process != null)
        {
            Shell.Stop(_process);
        }
    }

    public async Task Pause()
    {
        if (!Paused && Playing && _process != null)
        {
            await Shell.Pause(_process);

            Paused = true;
        }
    }
}
