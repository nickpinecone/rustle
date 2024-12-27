using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FFAudio;

public class FFRecorder
{
    public event EventHandler? RecordFinished = null;

    private Process? _process = null;

    public string ProgramPath { get; set; }
    public string AudioSystem { get; set; }
    public string Microphone { get; set; }

    public bool Recording { get; private set; }
    public bool Paused { get; private set; }

    public FFRecorder(string programPath, string audioSystem, string microphone = "default")
    {
        ProgramPath = programPath;
        AudioSystem = audioSystem;
        Microphone = microphone;
    }

    private void HandleFinished(object? sender, EventArgs e)
    {
        _process = null;

        Recording = false;
        Paused = false;

        RecordFinished?.Invoke(this, e);
    }

    public void Record(string path)
    {
        Stop();

        if (!Recording && _process == null)
        {
            _process = Shell.Create(ProgramPath, $"-y -loglevel error -f {AudioSystem} -i {Microphone} {path}");

            _process.Exited += HandleFinished;
            _process.Disposed += HandleFinished;
            _process.ErrorDataReceived += HandleFinished;

            Shell.Start(_process);

            Recording = true;
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

    public async Task Wait()
    {
        if (Recording && !Paused && _process != null)
        {
            await _process.WaitForExitAsync();
        }
    }

    public void Stop()
    {
        if (Recording && _process != null)
        {
            Shell.Stop(_process);

            _process = null;
            Recording = false;
            Paused = false;
        }
    }

    /// <summary>
    /// Uses <c>kill</c> to pause the recorder
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Thrown when used on Windows
    /// </exception>
    public async Task Pause()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (!Paused && Recording && _process != null)
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
