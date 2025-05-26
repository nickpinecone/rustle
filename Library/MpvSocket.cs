using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using Rustle.Library.Commands;

namespace Rustle.Library;

internal class MpvSocket
{
    private static ResiliencePipeline DefaultRetry =>
        new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions())
            .AddTimeout(TimeSpan.FromSeconds(5))
            .Build();

    private readonly string _name = $"mpv-socket-{Guid.NewGuid()}";
    private readonly CancellationTokenSource _tokenSource = new();

    private readonly Socket _socket = new(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
    private readonly ConcurrentDictionary<int, TaskCompletionSource<string>> _commandResponses = new();

    public string GetName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return $"/tmp/{_name}";
        }
        else
        {
            return @$"\\.\pipe\{_name}";
        }
    }

    public async Task CloseAsync()
    {
        await _tokenSource.CancelAsync();
    }

    public async Task ConnectAsync()
    {
        await DefaultRetry.ExecuteAsync(async token =>
        {
            if (!_socket.Connected)
            {
                await _socket.ConnectAsync(new UnixDomainSocketEndPoint(GetName()), token);
            }
        });

        _ = ReceiveLoopAsync();
    }

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

    private async Task ReceiveLoopAsync()
    {
        var buffer = new byte[4096];
        var leftover = string.Empty;

        try
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                var received = await DefaultRetry.ExecuteAsync(async _ =>
                    await _socket.ReceiveAsync(buffer, SocketFlags.None, _tokenSource.Token));

                if (received == 0) break;

                var data = leftover + Encoding.UTF8.GetString(buffer, 0, received);
                var messages = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                if (data.EndsWith('\n'))
                {
                    foreach (var message in messages)
                    {
                        ProcessMessage(message);
                    }

                    leftover = string.Empty;
                }
                else
                {
                    for (var i = 0; i < messages.Length - 1; i++)
                    {
                        ProcessMessage(messages[i]);
                    }

                    leftover = messages.Length > 0 ? messages[^1] : string.Empty;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Shutting down
        }
    }

    private void ProcessMessage(string message)
    {
        try
        {
            var json = JsonSerializer.Deserialize<MpvResponse>(message);

            if (json is not null && _commandResponses.TryRemove(json.RequestId, out var taskSource))
            {
                taskSource.TrySetResult(message);
            }
        }
        catch (JsonException)
        {
            // Invalid message
        }
    }

    public async Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command)
        where TCommand : MpvCommand
        where TResponse : MpvResponse
    {
        var taskSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        _commandResponses.TryAdd(command.RequestId, taskSource);
        
        await SendAsync(command);
        
        using var timeout = new CancellationTokenSource();
        timeout.CancelAfter(TimeSpan.FromSeconds(5));

        var responseRaw = await taskSource.Task.WaitAsync(timeout.Token);
        var response = JsonSerializer.Deserialize<TResponse>(responseRaw);
        
        return response!;
    }

    public async Task SendCommandAsync<TCommand>(TCommand command)
        where TCommand : MpvCommand
    {
        await SendCommandAsync<TCommand, MpvResponse>(command);
    }
}