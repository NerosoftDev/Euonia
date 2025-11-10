namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents the decorator for the request.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class RequestAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequestAttribute"/> class.
	/// </summary>
	/// <param name="responseType"></param>
	public RequestAttribute(Type responseType)
	{
		ResponseType = responseType;
	}

	/// <summary>
	/// Gets the response type for the request.
	/// </summary>
	public Type ResponseType { get; }
}