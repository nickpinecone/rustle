using System.Threading.Tasks;
using Rustle.Library.Commands;

namespace Rustle.Library.Socket;

internal interface IMpvSocket
{
    public string GetName();

    public Task ConnectAsync();

    public Task SendCommandAsync<TCommand>(TCommand command)
        where TCommand : MpvCommand;

    public Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command)
        where TCommand : MpvCommand
        where TResponse : MpvResponse;
}