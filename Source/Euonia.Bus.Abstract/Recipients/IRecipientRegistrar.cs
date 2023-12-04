namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Defines interface for message recipient registrar.
/// </summary>
public interface IRecipientRegistrar
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="registrations"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task RegisterAsync(IEnumerable<MessageRegistration> registrations, CancellationToken cancellationToken = default);
}
