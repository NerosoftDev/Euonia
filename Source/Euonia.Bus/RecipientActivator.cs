using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The background service to activate the recipients.
/// </summary>
public class RecipientActivator : BackgroundService
{
	private readonly IServiceProvider _provider;

	/// <summary>
	/// Initializes a new instance of the <see cref="RecipientActivator"/> class.
	/// </summary>
	/// <param name="provider"></param>
	public RecipientActivator(IServiceProvider provider)
	{
		_provider = provider;
	}

	/// <inheritdoc/>
	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var registrations = Singleton<BusConfigurator>.Instance.Registrations;

		var registrars = _provider.GetServices<IRecipientRegistrar>();

		return Task.WhenAll(registrars.Select(x => x.RegisterAsync(registrations, stoppingToken)));
	}
}