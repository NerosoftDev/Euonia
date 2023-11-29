namespace Nerosoft.Euonia.Bus;

public interface IRecipientRegistrar
{
	Task RegisterAsync(IReadOnlyList<MessageRegistration> registrations, CancellationToken cancellationToken = default);
}
