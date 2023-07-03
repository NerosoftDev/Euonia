using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Interface IEventHandler
/// Implements the <see cref="IMessageHandler" />
/// </summary>
/// <seealso cref="IMessageHandler" />
public interface IEventHandler : IMessageHandler
{
}

/// <summary>
/// Interface IEventHandler
/// Implements the <see cref="IEventHandler" />
/// Implements the <see cref="IMessageHandler{TEvent}" />
/// </summary>
/// <typeparam name="TEvent">The type of the t event.</typeparam>
/// <seealso cref="IEventHandler" />
/// <seealso cref="IMessageHandler{TEvent}" />
public interface IEventHandler<in TEvent> : IEventHandler, IMessageHandler<TEvent>
    where TEvent : IEvent
{
}