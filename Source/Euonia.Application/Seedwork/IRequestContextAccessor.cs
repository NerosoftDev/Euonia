using System.Security.Claims;
using Microsoft.Extensions.Primitives;

namespace Nerosoft.Euonia.Application;

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

public delegate ClaimsPrincipal GetRequestUserDelegate();

public delegate IDictionary<object, object> GetRequestItemsDelegate();

public delegate string GetRequestTraceIdentifierDelegate();

public delegate IServiceProvider GetRequestServicesDelegate();

public delegate CancellationToken GetRequestAbortedDelegate();

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

    public DelegateRequestContextAccessor()
    {
    }

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

    public ClaimsPrincipal User => _getUser?.Invoke();

    public IDictionary<object, object> Items => _getItems?.Invoke();

    public string TraceIdentifier => _getTraceIdentifier?.Invoke();

    public IServiceProvider RequestServices => _getServices?.Invoke();

    public CancellationToken RequestAborted => _getAborted?.Invoke() ?? default;

    public IDictionary<string, StringValues> RequestHeaders => _getHeaders?.Invoke();
}