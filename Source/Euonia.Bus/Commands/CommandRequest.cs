using MediatR;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResult"></typeparam>
public class CommandRequest<TCommand, TResult> : IRequest<TResult>
    where TCommand : ICommand
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="waitResponse"></param>
    public CommandRequest(TCommand command, bool waitResponse)
    {
        Command = command;
        WaitResponse = waitResponse;
    }

    internal TCommand Command { get; }

    internal bool WaitResponse { get; }
}