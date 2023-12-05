using System.Net;

namespace Nerosoft.Euonia.Core;

/// <summary>
/// Represents errors that occur when HTTP status code is not 200.
/// </summary>
[Serializable]
public class HttpStatusException : Exception
{
    private readonly HttpStatusCode _statusCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpStatusException"/> class.
    /// </summary>
    /// <param name="statusCode"></param>
    public HttpStatusException(HttpStatusCode statusCode)
        : base(statusCode.ToString())
    {
        _statusCode = statusCode;
    }

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public HttpStatusException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		_statusCode = (HttpStatusCode)info.GetInt32(nameof(StatusCode));
	} 
#endif

	/// <summary>
	/// Initializes a new instance of the <see cref="HttpStatusException"/> class.
	/// </summary>
	/// <param name="statusCode"></param>
	/// <param name="message"></param>
	public HttpStatusException(HttpStatusCode statusCode, string message)
        : base(message)
    {
        _statusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpStatusException"/> class.
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public HttpStatusException(HttpStatusCode statusCode, string message, Exception innerException)
        : base(message, innerException)
    {
        _statusCode = statusCode;
    }

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue(nameof(StatusCode), (int)_statusCode, typeof(int));
	} 
#endif

	/// <summary>
	/// Gets the HTTP status code.
	/// </summary>
	public virtual HttpStatusCode StatusCode => _statusCode;
}