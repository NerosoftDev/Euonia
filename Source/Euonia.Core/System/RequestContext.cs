using System.Net;
using System.Security.Claims;

namespace System;

/// <summary>
/// Contains information about the current request.
/// </summary>
public sealed class RequestContext
{
	/// <summary>
	/// Gets or sets a unique identifier to represent the connection.
	/// </summary>
	public string ConnectionId { get; set; }

	/// <summary>
	/// Gets or sets the IP address of the remote target. Can be null.
	/// </summary>
	public IPAddress RemoteIpAddress { get; set; }

	/// <summary>
	/// Gets or sets the port of the remote target.
	/// </summary>
	public int RemotePort { get; }

	/// <summary>
	/// Gets a value indicating whether the request is a WebSocket establishment request.
	/// </summary>
	public bool IsWebSocketRequest { get; }

	/// <summary>
	/// Gets or sets the user for this request.
	/// </summary>
	public ClaimsPrincipal User { get; set; }

	/// <summary>
	/// Gets the request headers.
	/// </summary>
	/// <returns>The request headers.</returns>
	public IDictionary<string, string> RequestHeaders { get; set; }

	/// <summary>
	/// Gets the Authorization HTTP header.
	/// </summary>
	public string Authorization => RequestHeaders?.TryGetValue(nameof(Authorization)) ?? default;

	/// <summary>
	/// Gets or sets the Request-Id HTTP header.
	/// </summary>
	public string RequestId => RequestHeaders?.TryGetValue("Request-Id") ?? default;

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
