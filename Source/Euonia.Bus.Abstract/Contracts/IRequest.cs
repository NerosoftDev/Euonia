namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Defines a request message with response.
/// </summary>
/// <typeparamref name="TResponse">The response message type.</typeparamref>
public interface IRequest<out TResponse>
{
}