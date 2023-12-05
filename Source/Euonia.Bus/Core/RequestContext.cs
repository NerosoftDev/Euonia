using System.Security.Claims;
using Microsoft.Extensions.Primitives;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Contains information about the current request.
/// </summary>
public sealed class RequestContext
{
	/// <summary>
	/// Gets or sets a unique identifier to represent the connection.
	/// </summary>
	public string ConnectionId { get; }

	/// <summary>
	/// Gets or sets the user for this request.
	/// </summary>
	public ClaimsPrincipal User { get; set; }

	/// <summary>
	/// Gets the request headers.
	/// </summary>
	/// <returns>The request headers.</returns>
	public IDictionary<string, StringValues> Headers { get; set; }

	/// <summary>
	/// Gets the Authorization HTTP header.
	/// </summary>
	public string Authorization
	{
		get
		{
			if (Headers == null)
			{
				return null;
			}

			return Headers.TryGetValue(nameof(Authorization), out var value) ? value : string.Empty;
		}
	}

	/// <summary>
	/// Gets or sets the <see cref="IServiceProvider"/> that provides access to the request's service container.
	/// </summary>
	public IServiceProvider RequestServices { get; set; }

	/// <summary>
	/// Notifies when the connection underlying this request is aborted and thus request operations should be
	/// cancelled.
	/// </summary>
	public CancellationToken RequestAborted { get; set; }

	/// <summary>
	/// Gets or sets a unique identifier to represent this request in trace logs.
	/// </summary>
	public string TraceIdentifier { get; set; }
}
