namespace Nerosoft.Euonia.Security;

/// <summary>
/// Exception thrown when provided credentials are incorrect.
/// </summary>
public class CredentialIncorrectException : CredentialException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CredentialIncorrectException"/> class for the specified credential.
	/// </summary>
	/// <param name="credential">The credential object that was determined to be incorrect.</param>
	public CredentialIncorrectException(object credential)
		: base(credential)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CredentialIncorrectException"/> class for the specified credential with a custom error message.
	/// </summary>
	/// <param name="credential">The credential object that was determined to be incorrect.</param>
	/// <param name="message">The message that describes the error.</param>
	public CredentialIncorrectException(object credential, string message)
		: base(credential, message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CredentialIncorrectException"/> class for the specified credential with a custom error message and an inner exception.
	/// </summary>
	/// <param name="credential">The credential object that was determined to be incorrect.</param>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="innerException">The exception that caused the current exception, if any.</param>
	public CredentialIncorrectException(object credential, string message, Exception innerException)
		: base(credential, message, innerException)
	{
	}
}