namespace Nerosoft.Euonia.Security;

/// <summary>
/// Exception thrown when a credential is not found during authentication or lookup.
/// Carries the credential object that was not found for diagnostics.
/// </summary>
public class CredentialNotFoundException : CredentialException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CredentialNotFoundException"/> class for the specified credential.
	/// </summary>
	/// <param name="credential">The credential object that could not be found.</param>
	public CredentialNotFoundException(object credential)
		: base(credential)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CredentialNotFoundException"/> class with a specified error message for the specified credential.
	/// </summary>
	/// <param name="credential">The credential object that could not be found.</param>
	/// <param name="message">The message that describes the error.</param>
	public CredentialNotFoundException(object credential, string message)
		: base(credential, message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CredentialNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception, for the specified credential.
	/// </summary>
	/// <param name="credential">The credential object that could not be found.</param>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.</param>
	public CredentialNotFoundException(object credential, string message, Exception innerException)
		: base(credential, message, innerException)
	{
	}
}