using MediatR;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

public class CommandRequest<TCommand, TResult> : IRequest<TResult>
    where TCommand : ICommand
{
    public CommandRequest(TCommand command, bool waitResponse)
    {
        Command = command;
        WaitResponse = waitResponse;
    }

    internal TCommand Command { get; }

    internal bool WaitResponse { get; }
}