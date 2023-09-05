using System.Security.Claims;
using Microsoft.Extensions.Primitives;

namespace Nerosoft.Euonia.Application;

/// <summary>
/// Defines the interface of a request context accessor.
/// </summary>
public interface IRequestContextAccessor
{
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
/// An implementation of <see cref="IRequestContextAccessor"/> using delegate methods.
/// </summary>
public class DelegateRequestContextAccessor : IRequestContextAccessor
{
    private readonly GetRequestUserDelegate _getUser;
    private readonly GetRequestItemsDelegate _getItems;
    private readonly GetRequestTraceIdentifierDelegate _getTraceIdentifier;
    private readonly GetRequestServicesDelegate _getServices;
    private readonly GetRequestAbortedDelegate _getAborted;
    private readonly GetRequestHeadersDelegate _getHeaders;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateRequestContextAccessor"/> class.
    /// </summary>
    public DelegateRequestContextAccessor()
    {
    }

    /// <inheritdoc />
    public DelegateRequestContextAccessor(GetRequestUserDelegate getUser, GetRequestItemsDelegate getItems, GetRequestTraceIdentifierDelegate getTraceIdentifier, GetRequestServicesDelegate getServices, GetRequestAbortedDelegate getAborted, GetRequestHeadersDelegate getHeaders)
        : this()
    {
        _getUser = getUser;
        _getItems = getItems;
        _getTraceIdentifier = getTraceIdentifier;
        _getServices = getServices;
        _getAborted = getAborted;
        _getHeaders = getHeaders;
    }

    /// <summary>
    /// Gets the authenticated user information for the current request.
    /// </summary>
    public ClaimsPrincipal User => _getUser?.Invoke();

    /// <summary>
    /// Gets the items for the current request.
    /// </summary>
    public IDictionary<object, object> Items => _getItems?.Invoke();

    /// <summary>
    /// Gets the request trace identifier.
    /// </summary>
    public string TraceIdentifier => _getTraceIdentifier?.Invoke();

    /// <summary>
    /// Gets the injected service provider for the current request.
    /// </summary>
    public IServiceProvider RequestServices => _getServices?.Invoke();

    /// <summary>
    /// Gets the request cancellation token.
    /// </summary>
    public CancellationToken RequestAborted => _getAborted?.Invoke() ?? default;

    /// <summary>
    /// Gets the request headers.
    /// </summary>
    public IDictionary<string, StringValues> RequestHeaders => _getHeaders?.Invoke();
}