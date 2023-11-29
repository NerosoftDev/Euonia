using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The background service to active the recipients.
/// </summary>
/// <param name="provider"></param>
public class RecipientActivator(IServiceProvider provider) : BackgroundService
{
	private readonly IServiceProvider _provider = provider;

	/// <inheritdoc/>
	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var registrations = Singleton<BusConfigurator>.Instance.Registrations;

		var registrars = _provider.GetServices<IRecipientRegistrar>();

		return Task.WhenAll(registrars.Select(x => x.RegisterAsync(registrations, stoppingToken)));
	}
}
