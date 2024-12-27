using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FFAudio;

public class FFPlayer
{
    public event EventHandler? PlaybackFinished = null;

    private Process? _process = null;

    public string ProgramPath { get; set; }
    public bool Playing { get; private set; }
    public bool Paused { get; private set; }

    public FFPlayer(string programPath)
    {
        ProgramPath = programPath;
    }

    private void HandleFinished(object? sender, EventArgs e)
    {
        _process = null;

        Playing = false;
        Paused = false;

        PlaybackFinished?.Invoke(this, e);
    }

    public void Play(string path)
    {
        Stop();

        if (!Playing && _process == null)
        {
            _process = Shell.Create(ProgramPath, $"-nodisp -loglevel error -autoexit {path}");

            _process.EnableRaisingEvents = true;
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

            _process = null;
            Playing = false;
            Paused = false;
        }
    }

    public async Task Resume()
    {
        if (Paused && _process != null)
        {
            await Shell.Resume(_process);

            Paused = false;
        }
    }

    /// <summary>
    /// Uses <c>kill</c> to pause the player
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown when used on Windows
    /// </exception>
    public async Task Pause()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (!Paused && Playing && _process != null)
            {
                await Shell.Pause(_process);

                Paused = true;
            }
        }
        else
        {
            throw new NotImplementedException("Pause not implemented on Windows");
        }
    }
}
