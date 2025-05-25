using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using Rustle.Library.Commands;

namespace Rustle.Library.Socket;

internal class WindowsSocket : IMpvSocket
{
    private readonly string _name = @$"\\.\pipe\mpv-socket-{Guid.NewGuid()}";
    private readonly NamedPipeClientStream? _pipeClient;

    public string GetName()
    {
        return _name;
    }

    public Task CloseAsync()
    {
        throw new NotImplementedException();
    }

    public Task ConnectAsync()
    {
        throw new NotImplementedException();
    }

    public Task SendCommandAsync<TCommand>(TCommand command)
        where TCommand : MpvCommand
    {
        throw new NotImplementedException();
    }

    public Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command)
        where TCommand : MpvCommand
        where TResponse : MpvResponse
    {
        throw new NotImplementedException();
    }
}