using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Rayer.Library.Commands;

namespace Rayer.Library.Socket;

internal class UnixSocket : IMpvSocket
{
    private readonly string _name = $"/tmp/mpv-socket-{Guid.NewGuid()}";
    private System.Net.Sockets.Socket? _socket;

    public string GetName()
    {
        return _name;
    }

    [MemberNotNull(nameof(_socket))]
    public async Task ConnectAsync()
    {
        _socket = new System.Net.Sockets.Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP)
        {
            Blocking = false,
        };

        var retries = 0;
        while (!_socket.Connected && retries < 5)
        {
            retries += 1;

            await Task.Delay(100 * retries);

            try
            {
                await _socket.ConnectAsync(new UnixDomainSocketEndPoint(_name));
            }
            catch
            {
                // ignored
            }
        }

        if (!_socket.Connected)
        {
            throw new Exception("Could not establish connection to the socket");
        }
    }

    [MemberNotNull(nameof(_socket))]
    public async Task SendCommandAsync<TCommand>(TCommand command)
        where TCommand : MpvCommand
    {
        ArgumentNullException.ThrowIfNull(_socket);

        var json = JsonSerializer.Serialize(command);
        var bytes = Encoding.UTF8.GetBytes(json + "\n");

        await _socket.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
    }

    [MemberNotNull(nameof(_socket))]
    public async Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command)
        where TCommand : MpvCommand
        where TResponse : MpvResponse
    {
        await SendCommandAsync(command);

        var responseBuffer = new StringBuilder();
        var buffer = new byte[4096];

        while (_socket.Available > 0)
        {
            var received = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            responseBuffer.Append(Encoding.UTF8.GetString(buffer, 0, received));
        }

        var response = responseBuffer
            .ToString()
            .Split("\n")
            .Where(res => !res.Contains("event"))
            .Select(res => JsonSerializer.Deserialize<TResponse>(res))
            .FirstOrDefault(res => res?.RequestId == command.RequestId);

        if (response is null)
        {
            throw new Exception("Wrong response type specified");
        }

        return response;
    }
}