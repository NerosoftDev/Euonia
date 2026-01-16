using System.Security.Authentication;

namespace Nerosoft.Euonia.Security;

/// <summary>
/// Exception thrown for account-related errors during authentication.
/// </summary>
public abstract class AccountException : AuthenticationException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AccountException"/> class for the specified account identifier.
	/// </summary>
	/// <param name="identity">The account identifier associated with the error.</param>
	protected AccountException(string identity)
	{
		Identity = identity;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AccountException"/> class with a specified error message.
	/// </summary>
	/// <param name="identity">The account identifier associated with the error.</param>
	/// <param name="message">The message that describes the error.</param>
	protected AccountException(string identity, string message)
		: base(message)
	{
		Identity = identity;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AccountException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
	/// </summary>
	/// <param name="identity">The account identifier associated with the error.</param>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.</param>
	protected AccountException(string identity, string message, Exception innerException)
		: base(message, innerException)
	{
		Identity = identity;
	}

	/// <summary>
	/// Gets the account identifier that caused the exception.
	/// </summary>
	public string Identity { get; }
	
	/// <summary>
	/// Gets a dictionary for storing additional details or metadata about the account error.
	/// Keys are strings and values are arbitrary objects.
	/// </summary>
	public virtual Dictionary<string, object> Details { get; } = new();
}