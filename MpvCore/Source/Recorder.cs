using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FFAudio;

public class FFRecorder
{
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
    }

    public async Task Record(string path)
    {
        if (Paused && _process != null)
        {
            await Shell.Resume(_process);

            Paused = false;
        }

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
        }
    }

    public async Task Pause()
    {
        if (!Paused && Recording && _process != null)
        {
            await Shell.Pause(_process);

            Paused = true;
        }
    }
}
