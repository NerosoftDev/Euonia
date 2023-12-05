using System.Net;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Nerosoft.Euonia.Application;

/// <summary>
/// Defines the interface of a request context accessor.
/// </summary>
public interface IRequestContextAccessor
{
	/// <summary>
	/// Gets a unique identifier to represent the connection.
	/// </summary>
	string ConnectionId { get; }

	/// <summary>
	/// Gets the IP address of the remote target. Can be null.
	/// </summary>
	IPAddress RemoteIpAddress { get; }

	/// <summary>
	/// Gets the port of the remote target.
	/// </summary>
	int RemotePort { get; }

	/// <summary>
	/// 
	/// </summary>
	bool IsWebSocketRequest { get; }

	/// <summary>
	/// Gets the authenticated user information for the current request.
	/// </summary>
	ClaimsPrincipal User { get; }

	/// <summary>
	/// Gets the items for the current request.
	/// </summary>
	IDictionary<object, object> Items { get; }

	/// <summary>
	/// Gets the request trace identifier.
	/// </summary>
	string TraceIdentifier { get; }

	/// <summary>
	/// Gets the injected service provider for the current request.
	/// </summary>
	IServiceProvider RequestServices { get; }

	/// <summary>
	/// Gets the request cancellation token.
	/// </summary>
	CancellationToken RequestAborted { get; }

	/// <summary>
	/// Gets the request headers.
	/// </summary>
	IDictionary<string, StringValues> RequestHeaders { get; }

	/// <summary>
	/// Gets the Authorization HTTP header.
	/// </summary>
	StringValues Authorization => RequestHeaders?.TryGetValue(nameof(Authorization)) ?? default;

	/// <summary>
	/// Gets or sets the Request-Id HTTP header.
	/// </summary>
	StringValues RequestId => RequestHeaders?.TryGetValue("Request-Id") ?? default;
}

/// <summary>
/// Gets the authenticated user information for the current request.
/// </summary>
public delegate ClaimsPrincipal GetRequestUserDelegate();

/// <summary>
/// Gets the items for the current request.
/// </summary>
public delegate IDictionary<object, object> GetRequestItemsDelegate();

/// <summary>
/// Gets the request trace identifier.
/// </summary>
public delegate string GetRequestTraceIdentifierDelegate();

/// <summary>
/// Gets the injected service provider for the current request.
/// </summary>
public delegate IServiceProvider GetRequestServicesDelegate();

/// <summary>
/// Gets the request cancellation token.
/// </summary>
public delegate CancellationToken GetRequestAbortedDelegate();

/// <summary>
/// Gets the request headers.
/// </summary>
public delegate IDictionary<string, StringValues> GetRequestHeadersDelegate();

/// <summary>
/// Define delegation to get connection information of current request.
/// </summary>
/// <returns></returns>
public delegate Tuple<string, IPAddress, int, bool> GetConnectionInfoDelegate();

/// <summary>
/// An implementation of <see cref="IRequestContextAccessor"/> using delegate methods.
/// </summary>
public class DelegateRequestContextAccessor : IRequestContextAccessor
{

	private readonly IServiceProvider _provider;

	/// <summary>
	/// Initializes a new instance of the <see cref="DelegateRequestContextAccessor"/> class.
	/// </summary>
	public DelegateRequestContextAccessor(IServiceProvider provider)
	{
		_provider = provider;
		var (connectionId, remoteIpAddress, remotePort, isWebSocketRequest) = provider.GetService<GetConnectionInfoDelegate>()?.Invoke() ?? default;
		ConnectionId = connectionId;
		RemoteIpAddress = remoteIpAddress;
		RemotePort = remotePort;
		IsWebSocketRequest = isWebSocketRequest;
	}

	/// <inheritdoc/>
	public string ConnectionId { get; }

	/// <inheritdoc/>
	public IPAddress RemoteIpAddress { get; }

	/// <inheritdoc/>
	public int RemotePort { get; }

	/// <inheritdoc/>
	public bool IsWebSocketRequest { get; }

	/// <summary>
	/// Gets the authenticated user information for the current request.
	/// </summary>
	public ClaimsPrincipal User => _provider.GetService<GetRequestUserDelegate>()?.Invoke();

	/// <summary>
	/// Gets the items for the current request.
	/// </summary>
	public IDictionary<object, object> Items => _provider.GetService<GetRequestItemsDelegate>()?.Invoke();

	/// <summary>
	/// Gets the request trace identifier.
	/// </summary>
	public string TraceIdentifier => _provider.GetService<GetRequestTraceIdentifierDelegate>()?.Invoke();

	/// <summary>
	/// Gets the injected service provider for the current request.
	/// </summary>
	public IServiceProvider RequestServices => _provider.GetService<GetRequestServicesDelegate>()?.Invoke();

	/// <summary>
	/// Gets the request cancellation token.
	/// </summary>
	public CancellationToken RequestAborted => _provider.GetService<GetRequestAbortedDelegate>()?.Invoke() ?? default;

	/// <summary>
	/// Gets the request headers.
	/// </summary>
	public IDictionary<string, StringValues> RequestHeaders => _provider.GetService<GetRequestHeadersDelegate>()?.Invoke();
}