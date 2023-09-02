using System.Net;

namespace Nerosoft.Euonia.Core;

/// <inheritdoc />
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class HttpStatusCodeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpStatusCodeAttribute"/> class.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    public HttpStatusCodeAttribute(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpStatusCodeAttribute"/> class.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    public HttpStatusCodeAttribute(int statusCode)
        : this((HttpStatusCode)statusCode)
    {
    }

    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; }
}