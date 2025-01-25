using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using CliWrap;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace FFAudio;

public class Audio
{
    public event EventHandler? PlaybackFinished = null;

    public static string ProgramPath { get; set; }

    public string Filename { get; private set; }
    public bool Paused { get; private set; }
    public bool Playing { get; private set; }
    public PlaybackOptions Options { get; private set; }

    private CancellationTokenSource _tokenSource;
    private int? _processId;
    private CancellationToken _token;

    static Audio()
    {
        ProgramPath = "ffplay";
    }

    public Audio(string filename, PlaybackOptions? options = null)
    {
        Filename = filename;
        Paused = false;
        Playing = false;

        Options = options ?? new PlaybackOptions();
        _tokenSource = new CancellationTokenSource();
        _token = _tokenSource.Token;
    }

    private void Reset()
    {
        Paused = false;
        Playing = false;

        _tokenSource = new CancellationTokenSource();
        _token = _tokenSource.Token;
    }

    public async Task Play()
    {
        if (Playing)
        {
            throw new InvalidOperationException("Audio is already being played");
        }

        Playing = true;

        var args = Options.GetArguments();
        args.Add(Filename);

        var builder = new StringBuilder();
        var command = Cli.Wrap(ProgramPath)
                          .WithArguments(string.Join(" ", args))
                          .WithWorkingDirectory(Directory.GetCurrentDirectory())
                          .WithStandardErrorPipe(PipeTarget.ToStringBuilder(builder))
                          .ExecuteAsync(_token);

        _processId = command.ProcessId;
        var result = await command;
        var output = builder.ToString();

        Reset();

        if (!string.IsNullOrEmpty(output))
        {
            throw new Exception(output);
        }

        PlaybackFinished?.Invoke(this, EventArgs.Empty);
    }

    public void Stop()
    {
        _tokenSource.Cancel();
    }

    public async Task Pause()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (!Paused && Playing && _processId != null)
            {
                Paused = true;
                await Cli.Wrap("kill").WithArguments($"-TSTP {_processId}").ExecuteAsync();
            }
        }
        else
        {
            throw new NotImplementedException("Pause is not implemented on Windows");
        }
    }

    public async Task Resume()
    {
        if (Paused)
        {
            Paused = false;
            await Cli.Wrap("kill").WithArguments($"-CONT {_processId}").ExecuteAsync();
        }
    }
}

public class PlaybackOptions
{
    internal List<string> GetArguments()
    {
        var arguments = new List<string> { "-nodisp", "-loglevel error", "-autoexit" };

        return arguments;
    }
}
