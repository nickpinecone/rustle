using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CliWrap;

namespace Rayer.Library;

public interface IIpcSocket
{
    public string GetName();
    
    public Task ConnectAsync();
    public Task SendCommandAsync<TCommand>(TCommand command);
    public Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command);
}

public record PauseCommand()
{
    [JsonPropertyName("command")]
    public object[] Command { get; set; } = ["set_property", "pause", true];
}

public record ResumeCommand()
{
    [JsonPropertyName("command")]
    public object[] Command { get; set; } = ["set_property", "pause", false];
}

public record PauseResponse()
{
    [JsonPropertyName("error")]
    public required string Error { get; set; }
    
    [JsonPropertyName("data")]
    public required object Data { get; set; }
}

public class UnixSocket : IIpcSocket
{
    private readonly string _name = $"/tmp/mpvsocket.sock";
    private Socket? _socket;
    
    public string GetName()
    {
        return _name;
    }

    public async Task ConnectAsync()
    {
        _socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP)
        {
            Blocking = false,
        };
        await _socket.ConnectAsync(new UnixDomainSocketEndPoint(_name));
        _socket.SendBufferSize = 0;
    }

    public async Task SendCommandAsync<TCommand>(TCommand command)
    {
        ArgumentNullException.ThrowIfNull(_socket);

        var json = JsonSerializer.Serialize(command);
        var bytes = Encoding.UTF8.GetBytes(json + "\n");
        
        // var cmd = "{\"command\": [\"set_property\", \"pause\", false]}";
        // using var ms = new MemoryStream(Encoding.UTF8.GetBytes(cmd));
        // await ms.CopyToAsync(new NetworkStream(_socket));
        
        // var process = new Process()
        // {
        //     StartInfo = new ProcessStartInfo
        //     {
        //         FileName = "socat",
        //         Arguments = "- UNIX-CONNECT:/tmp/mpvsocket.sock",
        //         RedirectStandardInput = true,
        //         UseShellExecute = false
        //     }
        // };
        // process.Start();
        //
        // await process.StandardInput.WriteLineAsync("{\"command\":[\"set_property\", \"pause\", true]}");
        await _socket.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
    }

    public async Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command)
    {
        ArgumentNullException.ThrowIfNull(_socket);
        
        var json = JsonSerializer.Serialize(command);
        var bytes = Encoding.UTF8.GetBytes(json);
        
        await _socket.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

        var responseBuffer = new StringBuilder();
        var buffer = new byte[4096];
        
        while(_socket.Available > 0)
        {
            var received = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            responseBuffer.Append(Encoding.UTF8.GetString(buffer, 0, received));
        } 

        var response  = JsonSerializer.Deserialize<TResponse>(responseBuffer.ToString());

        if (response is null)
        {
            throw new Exception("Wrong response type specified");
        }

        return response;
    }
}

public class WindowsSocket : IIpcSocket
{
    private readonly string _name = @$"\\.\pipe\mpv-socket-{Guid.NewGuid()}";
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