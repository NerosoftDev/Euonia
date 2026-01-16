namespace Nerosoft.Euonia.Security;

/// <summary>
/// Exception thrown when an account has expired and can no longer be used for authentication or access.
/// Carries the identity of the expired account for diagnostics.
/// </summary>
public class AccountExpiredException : AccountException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AccountExpiredException"/> class for the specified identity.
	/// </summary>
	/// <param name="identity">The identity (e.g., username or account id) of the expired account.</param>
	public AccountExpiredException(string identity)
		: base(identity)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AccountExpiredException"/> class with a specified error message for the specified identity.
	/// </summary>
	/// <param name="identity">The identity (e.g., username or account id) of the expired account.</param>
	/// <param name="message">A message that describes the error.</param>
	public AccountExpiredException(string identity, string message)
		: base(identity, message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AccountExpiredException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception, for the specified identity.
	/// </summary>
	/// <param name="identity">The identity (e.g., username or account id) of the expired account.</param>
	/// <param name="message">A message that describes the error.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.</param>
	public AccountExpiredException(string identity, string message, Exception innerException)
		: base(identity, message, innerException)
	{
	}
}