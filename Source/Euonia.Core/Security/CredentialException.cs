using System.Security.Authentication;

namespace Nerosoft.Euonia.Security;

/// <summary>
/// Base exception type for errors related to credentials used during authentication.
/// Carries the credential object that caused the error and an optional details dictionary for extra metadata.
/// </summary>
public abstract class CredentialException : AuthenticationException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CredentialException"/> class for the specified credential.
	/// </summary>
	/// <param name="credential">The credential object associated with the error.</param>
	protected CredentialException(object credential)
	{
		Credential = credential;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CredentialException"/> class with a specified error message.
	/// </summary>
	/// <param name="credential">The credential object associated with the error.</param>
	/// <param name="message">The message that describes the error.</param>
	protected CredentialException(object credential, string message)
		: base(message)
	{
		Credential = credential;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CredentialException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
	/// </summary>
	/// <param name="credential">The credential object associated with the error.</param>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.</param>
	protected CredentialException(object credential, string message, Exception innerException)
		: base(message, innerException)
	{
		Credential = credential;
	}

	/// <summary>
	/// Gets the credential object that caused the exception.
	/// </summary>
	public object Credential { get; }

	/// <summary>
	/// Gets a dictionary for storing additional details or metadata about the credential error.
	/// Keys are strings and values are arbitrary objects.
	/// </summary>
	public virtual Dictionary<string, object> Details { get; } = new();
}