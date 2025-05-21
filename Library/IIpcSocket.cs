using System;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rayer.Library;

public interface IIpcSocket
{
    public string GetName();
    
    public Task ConnectAsync();
    public Task SendCommandAsync<TCommand>(TCommand command);
    public Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command);
}

public class UnixSocket : IIpcSocket
{
    private readonly string _name = $"mpv-socket-{Guid.NewGuid()}";
    private readonly Socket? _socket;

    public string GetName()
    {
        return _name;
    }

    public Task ConnectAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task SendCommandAsync<TCommand>(TCommand command)
    {
        var json = JsonSerializer.Serialize(command);

        return Task.CompletedTask;
    }

    public Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command)
    {
        throw new System.NotImplementedException();
    }
}

public class WindowsSocket : IIpcSocket
{
    private readonly string _name = $"mpv-socket-{Guid.NewGuid()}";
    private readonly NamedPipeClientStream? _pipeClient;
    
    public string GetName()
    {
        return _name;
    }

    public Task ConnectAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task SendCommandAsync<TCommand>(TCommand command)
    {
        throw new System.NotImplementedException();
    }

    public Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command)
    {
        throw new System.NotImplementedException();
    }
}