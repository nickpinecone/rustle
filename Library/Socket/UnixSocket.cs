using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using Rustle.Library.Commands;

namespace Rustle.Library.Socket;

internal class UnixSocket : IMpvSocket
{
    private readonly string _name = $"/tmp/mpv-socket-{Guid.NewGuid()}";
    private System.Net.Sockets.Socket? _socket;

    private static ResiliencePipeline DefaultRetry =>
        new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions())
            .AddTimeout(TimeSpan.FromSeconds(5))
            .Build();

    public string GetName()
    {
        return _name;
    }

    public Task CloseAsync()
    {
        if (_socket is not null)
        {
            _socket.Close();
            _socket = null;
        }

        return Task.CompletedTask;
    }

    [MemberNotNull(nameof(_socket))]
    public async Task ConnectAsync()
    {
        await CloseAsync();
        
        _socket = new System.Net.Sockets.Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);

        await DefaultRetry.ExecuteAsync(async token =>
        {
            if (!_socket.Connected)
            {
                await _socket.ConnectAsync(new UnixDomainSocketEndPoint(_name), token);
            }
        });
    }

    [MemberNotNull(nameof(_socket))]
    private async Task SendAsync<TCommand>(TCommand command)
        where TCommand : MpvCommand
    {
        ArgumentNullException.ThrowIfNull(_socket);

        var json = JsonSerializer.Serialize(command);
        var bytes = Encoding.UTF8.GetBytes(json + "\n");

        await DefaultRetry.ExecuteAsync(async token =>
        {
            await _socket.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None, token);
        });
    }

    [MemberNotNull(nameof(_socket))]
    private async Task<TResponse> ReceiveAsync<TResponse>(int requestId)
        where TResponse : MpvResponse
    {
        ArgumentNullException.ThrowIfNull(_socket);

        var responseBuffer = new StringBuilder();
        var buffer = new byte[4096];

        while (_socket.Available > 0)
        {
            await DefaultRetry.ExecuteAsync(async token =>
            {
                var received = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None, token);
                responseBuffer.Append(Encoding.UTF8.GetString(buffer, 0, received));
            });
        }
        
        Console.WriteLine(responseBuffer.ToString());

        var response = responseBuffer
            .ToString()
            .Split("\n")
            .Where(res => !res.Contains("event"))
            .Select(res =>
            {
                try
                {
                    return JsonSerializer.Deserialize<TResponse>(res);
                }
                catch
                {
                    return null;
                }
            })
            .FirstOrDefault(res => res?.RequestId == requestId);

        if (response is null)
        {
            throw new Exception("Wrong response type specified");
        }

        return response;
    }

    public async Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command)
        where TCommand : MpvCommand
        where TResponse : MpvResponse
    {
        await SendAsync(command);

        return await DefaultRetry.ExecuteAsync(async _ => await ReceiveAsync<TResponse>(command.RequestId));
    }

    public async Task SendCommandAsync<TCommand>(TCommand command)
        where TCommand : MpvCommand
    {
        await SendCommandAsync<TCommand, MpvResponse>(command);
    }
}