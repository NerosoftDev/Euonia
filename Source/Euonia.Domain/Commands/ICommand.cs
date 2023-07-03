using MediatR;

namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The contract interface of command.
/// </summary>
/// <seealso cref="IMessage"/>
public interface ICommand : IMessage
{
}

/// <summary>
/// The contract interface of command.
/// </summary>
/// <seealso cref="IMessage"/>
/// <typeparam name="TResult"></typeparam>
public interface ICommand<out TResult> : IRequest<TResult>, ICommand
{
}