using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// A <see langword="class"/> for request messages that can receive multiple replies, which can either be used directly or through derived classes.
/// </summary>
/// <typeparam name="T">The type of request to make.</typeparam>
public class CollectionRequestMessage<T> : IEnumerable<T>
{
	private readonly List<T> _responses = new();

	/// <summary>
	/// Gets the message responses.
	/// </summary>
	public IReadOnlyCollection<T> Responses => _responses;

	/// <summary>
	/// Replies to the current request message.
	/// </summary>
	/// <param name="response">The response to use to reply to the request message.</param>
	public void Reply(T response)
	{
		_responses.Add(response);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public IEnumerator<T> GetEnumerator()
	{
		return _responses.GetEnumerator();
	}

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}