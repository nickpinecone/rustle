using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;

namespace Rustle.Library;

public class MpvPlayer : IAsyncDisposable
{
    private readonly CancellationTokenSource _tokenSource;
    private readonly MpvSocket _socket;

    private readonly string _mpvPath;
    private static int _uniqueId = 0;

    public event EventHandler<string>? MediaTitleChange = null;

    private string _prevUrl = string.Empty;
    public bool Playing { get; private set; } = false;

    public MpvPlayer(string? mpvPath = null)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            _mpvPath = mpvPath ?? "mpv";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _mpvPath = mpvPath ?? "mpv.exe";
        }
        else
        {
            throw new Exception("Unsupported operating system");
        }

        _socket = new MpvSocket();
        _tokenSource = new CancellationTokenSource();
    }

    public async Task Initialize()
    {
        _ = Cli.Wrap(_mpvPath)
            .WithArguments(["--no-video", "--idle", $"--input-ipc-server={_socket.GetName()}"])
            .ExecuteAsync(_tokenSource.Token);

        await _socket.ConnectAsync();

        _ = ProcessEvents();

        Interlocked.Increment(ref _uniqueId);
        await _socket.SendCommandAsync(new ObservePropertyCommand(_uniqueId, MpvProperties.MediaTitle));
    }

    private async Task ProcessEvents()
    {
        await foreach (var @event in _socket.Events)
        {
            switch (@event)
            {
                case PropertyChangeEvent propertyChange when propertyChange.Name == MpvProperties.MediaTitle:
                    MediaTitleChange?.Invoke(null, propertyChange.Data.GetString()!);
                    break;

                case EndFileEvent endFile when Playing:
                    await PlayAsync(_prevUrl);
                    break;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _tokenSource.CancelAsync();
        await _socket.CloseAsync();
    }

    public async Task PlayAsync(string url)
    {
        _prevUrl = url;
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
        await _socket.SendCommandAsync(new SetPropertyCommand(_uniqueId, MpvProperties.Pause, true));
    }

    public async Task ResumeAsync()
    {
        Interlocked.Increment(ref _uniqueId);
        await _socket.SendCommandAsync(new SetPropertyCommand(_uniqueId, MpvProperties.Pause, false));
    }

    public async Task<bool> GetPausedAsync()
    {
        Interlocked.Increment(ref _uniqueId);
        var response =
            await _socket.SendCommandAsync(new GetPropertyCommand(_uniqueId, MpvProperties.Pause));
        return response.Data.GetBoolean();
    }

    public async Task SetVolumeAsync(int volume)
    {
        Interlocked.Increment(ref _uniqueId);
        volume = int.Clamp(volume, 0, 100);
        await _socket.SendCommandAsync(new SetPropertyCommand(_uniqueId, MpvProperties.Volume, volume));
    }

    public async Task<int> GetVolumeAsync()
    {
        Interlocked.Increment(ref _uniqueId);
        var response =
            await _socket.SendCommandAsync(new GetPropertyCommand(_uniqueId, MpvProperties.Volume));
        return (int)response.Data.GetDouble();
    }

    public async Task<string> GetMediaTitleAsync()
    {
        Interlocked.Increment(ref _uniqueId);
        var response =
            await _socket.SendCommandAsync(new GetPropertyCommand(_uniqueId, MpvProperties.MediaTitle));
        return response.Data.GetString()!;
    }
}