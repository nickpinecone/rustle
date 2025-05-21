using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Rayer.Library;

public class MpvPlayer
{
    private IIpcSocket _socket;

    public MpvPlayer()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            _socket = new UnixSocket();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _socket = new WindowsSocket();
        }
        else
        {
            throw new Exception("Unsupported operating system");
        }
    }

    public Task PlayAsync(string filename)
    {
        return Task.CompletedTask;
    }
}