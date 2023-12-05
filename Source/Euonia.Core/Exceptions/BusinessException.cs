namespace Nerosoft.Euonia.Core;

/// <summary>
/// Represents errors that occur during business logic execution.
/// </summary>
[Serializable]
public class BusinessException : Exception
{
	private readonly string _code;

	/// <summary>
	/// Gets the business error code.
	/// </summary>
	public virtual string Code => _code;

	/// <inheritdoc />
	public BusinessException()
	{
	}

	/// <summary>
	/// Initialize a new instance of the <see cref="BusinessException"/> with error code.
	/// </summary>
	/// <param name="code">The error code.</param>
	public BusinessException(string code)
	{
		_code = code;
	}

	/// <summary>
	/// Initialize a new instance of the <see cref="BusinessException"/> with error code and message.
	/// </summary>
	/// <param name="code">The error code.</param>
	/// <param name="message">The error message.</param>
	public BusinessException(string code, string message)
		: base(message)
	{
		_code = code;
	}

	/// <summary>
	/// Initialize a new instance of the <see cref="BusinessException"/> with error code, message and inner exception.
	/// </summary>
	/// <param name="code">The error code.</param>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception.</param>
	public BusinessException(string code, string message, Exception innerException)
		: base(message, innerException)
	{
		_code = code;
	}

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public BusinessException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		_code = info.GetString(nameof(Code));
	}

	/// <inheritdoc />
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue(nameof(Code), _code, typeof(string));
	}
#endif
}