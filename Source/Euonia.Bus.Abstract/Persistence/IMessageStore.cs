namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Interface IMessageStore
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public interface IMessageStore : IDisposable
{
	/// <summary>
	/// Save the specified message to the current message store.
	/// </summary>
	/// <param name="message">The message to be saved.</param>
	/// <param name="context">The message context.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	Task SaveAsync<TMessage>(TMessage message, IMessageContext context, CancellationToken cancellationToken = default)
		where TMessage : class;
}