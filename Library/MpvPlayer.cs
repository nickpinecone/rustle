using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;

namespace Rayer.Library;

public class MpvPlayer
{
    private readonly IIpcSocket _socket;
    private readonly string _mpvPath;
    private CancellationTokenSource? _tokenSource;

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
        
        _ = Cli.Wrap(_mpvPath)
            .WithArguments(["--no-video", "--idle", $"--input-ipc-server={_socket.GetName()}", url])
            .ExecuteAsync(_tokenSource.Token);

        await Task.Delay(1000);
        await _socket.ConnectAsync();
    }

    public async Task PauseAsync()
    {
         await _socket.SendCommandAsync<PauseCommand>(new PauseCommand());
    }
    
    public async Task ResumeAsync()
    {
        await _socket.SendCommandAsync(new ResumeCommand());
    }

    public async Task StopAsync()
    {
        if (_tokenSource is not null)
        {
            await _tokenSource.CancelAsync();
            _tokenSource = null;
        }
    }
}