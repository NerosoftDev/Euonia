namespace Nerosoft.Euonia.Security;

/// <summary>
/// Exception thrown when an account with the specified identity cannot be found.
/// Carries the account identity for diagnostic purposes.
/// </summary>
public class AccountNotFoundException : AccountException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AccountNotFoundException"/> class for the specified identity.
	/// </summary>
	/// <param name="identity">The identity (for example, username or account id) of the account that was not found.</param>
	public AccountNotFoundException(string identity)
		: base(identity)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AccountNotFoundException"/> class for the specified identity with a custom error message.
	/// </summary>
	/// <param name="identity">The identity (for example, username or account id) of the account that was not found.</param>
	/// <param name="message">A message that describes the error.</param>
	public AccountNotFoundException(string identity, string message)
		: base(identity, message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AccountNotFoundException"/> class for the specified identity with a custom error message and an inner exception.
	/// </summary>
	/// <param name="identity">The identity (for example, username or account id) of the account that was not found.</param>
	/// <param name="message">A message that describes the error.</param>
	/// <param name="innerException">The exception that caused the current exception, if any.</param>
	public AccountNotFoundException(string identity, string message, Exception innerException)
		: base(identity, message, innerException)
	{
	}
}