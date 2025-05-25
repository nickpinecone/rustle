using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using Rustle.Library.Commands;
using Rustle.Library.Socket;

namespace Rustle.Library;

public class MpvPlayer : IAsyncDisposable
{
    private readonly CancellationTokenSource _tokenSource;
    private readonly IMpvSocket _socket;
    
    private readonly string _mpvPath;
    private static int _uniqueId = 0;

    public bool Playing { get; private set; } = false;

    public MpvPlayer(string? mpvPath = null)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            _mpvPath = mpvPath ?? "mpv";
            _socket = new UnixSocket();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _mpvPath = mpvPath ?? "mpv.exe";
            _socket = new WindowsSocket();
        }
        else
        {
            throw new Exception("Unsupported operating system");
        }

        _tokenSource = new CancellationTokenSource();
    }

    public async Task Initialize()
    {
        _ = Cli.Wrap(_mpvPath)
            .WithArguments(["--no-video", "--idle", $"--input-ipc-server={_socket.GetName()}"])
            .ExecuteAsync(_tokenSource.Token);

        await _socket.ConnectAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _tokenSource.CancelAsync();
        await _socket.CloseAsync();
        
        GC.SuppressFinalize(this);
    }

    public async Task PlayAsync(string url)
    {
        Interlocked.Increment(ref _uniqueId);
        await _socket.SendCommandAsync(new LoadfileCommand(_uniqueId, url));
        Playing = true;
    }

    public async Task StopAsync()
    {
        Interlocked.Increment(ref _uniqueId);
        await _socket.SendCommandAsync(new StopCommand(_uniqueId));
        Playing = false;
    }

    public async Task PauseAsync()
    {
        Interlocked.Increment(ref _uniqueId);
        await _socket.SendCommandAsync(new PauseCommand(_uniqueId));
    }

    public async Task ResumeAsync()
    {
        Interlocked.Increment(ref _uniqueId);
        await _socket.SendCommandAsync(new ResumeCommand(_uniqueId));
    }

    public async Task<bool> GetPausedAsync()
    {
        Interlocked.Increment(ref _uniqueId);
        var response =
            await _socket.SendCommandAsync<GetPauseCommand, GetPauseResponse>(new GetPauseCommand(_uniqueId));
        return response.IsPaused;
    }

    public async Task SetVolumeAsync(int volume)
    {
        Interlocked.Increment(ref _uniqueId);
        volume = int.Clamp(volume, 0, 100);
        await _socket.SendCommandAsync(new SetVolumeCommand(_uniqueId, volume));
    }

    public async Task<int> GetVolumeAsync()
    {
        Interlocked.Increment(ref _uniqueId);
        var response =
            await _socket.SendCommandAsync<GetVolumeCommand, GetVolumeResponse>(new GetVolumeCommand(_uniqueId));
        return (int)response.Volume;
    }

    public async Task<string> GetMediaTitleAsync()
    {
        Interlocked.Increment(ref _uniqueId);
        var response =
            await _socket.SendCommandAsync<GetMediaTitleCommand, GetMediaTitleResponse>(
                new GetMediaTitleCommand(_uniqueId)
            );
        return response.MediaTitle;
    }
}