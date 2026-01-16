namespace Nerosoft.Euonia.Security;

public class CredentialExpiredException : CredentialException
{
	public CredentialExpiredException(object credential)
		: base(credential)
	{
	}

	public CredentialExpiredException(object credential, string message)
		: base(credential, message)
	{
	}

	public CredentialExpiredException(object credential, string message, Exception innerException)
		: base(credential, message, innerException)
	{
	}
}