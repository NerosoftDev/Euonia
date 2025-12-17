using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The background service to activate the recipients.
/// </summary>
public class RecipientActivator : BackgroundService
{
	private readonly IServiceProvider _provider;
	private readonly IBusConfigurator _configurator;

	/// <summary>
	/// Initializes a new instance of the <see cref="RecipientActivator"/> class.
	/// </summary>
	/// <param name="provider"></param>
	/// <param name="configurator"></param>
	public RecipientActivator(IServiceProvider provider, IBusConfigurator configurator)
	{
		_provider = provider;
		_configurator = configurator;
	}

	/// <inheritdoc/>
	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var registrations = _configurator.Registrations;

		var registrars = _provider.GetServices<IRecipientRegistrar>();

		return Task.WhenAll(registrars.Select(x => x.RegisterAsync(registrations, stoppingToken)));
	}
}