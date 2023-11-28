namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents a queue message.
/// </summary>
public interface IQueue
{ }

/// <summary>
/// Represents a queue message.
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface IQueue<out TResponse> : IQueue
{ }