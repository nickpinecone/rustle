using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using Rayer.Library.Commands;
using Rayer.Library.Socket;

namespace Rayer.Library;

public class MpvPlayer
{
    private readonly IMpvSocket _socket;
    private readonly string _mpvPath;

    private CommandTask<CommandResult>? _playTask;
    private CancellationTokenSource? _tokenSource;

    private static int _uniqueId = 0;

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
    }

    public async Task PlayAsync(string url)
    {
        _tokenSource = new CancellationTokenSource();

        _playTask = Cli.Wrap(_mpvPath)
            .WithArguments(["--no-video", "--idle", $"--input-ipc-server={_socket.GetName()}", url])
            .ExecuteAsync(_tokenSource.Token);

        await _socket.ConnectAsync();
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

    public async Task WaitAsync()
    {
        if (_playTask is not null)
        {
            await _playTask;
        }
    }

    public async Task StopAsync()
    {
        if (_tokenSource is not null)
        {
            await _tokenSource.CancelAsync();
            
            _playTask = null;
            _tokenSource = null;
        }
    }
}