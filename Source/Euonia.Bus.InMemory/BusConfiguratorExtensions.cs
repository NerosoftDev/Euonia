using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Bus.InMemory;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Service bus extensions for <see cref="IBusConfigurator"/>.
/// </summary>
public static class BusConfiguratorExtensions
{
	/// <summary>
	/// Adds the in-memory message transporter.
	/// </summary>
	/// <param name="configurator"></param>
	/// <param name="configuration"></param>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static void UseInMemory(this IBusConfigurator configurator, Action<InMemoryBusOptions> configuration)
	{
		configurator.Service.Configure(configuration);
		configurator.Service.TryAddSingleton(provider =>
		{
			var options = provider.GetService<IOptions<InMemoryBusOptions>>()?.Value;
			if (options == null)
			{
				throw new InvalidOperationException("The in-memory message dispatcher options is not configured.");
			}

			IMessenger messenger = options.MessengerReference switch
			{
				MessengerReferenceType.StrongReference => StrongReferenceMessenger.Default,
				MessengerReferenceType.WeakReference => WeakReferenceMessenger.Default,
				_ => throw new ArgumentOutOfRangeException(nameof(options.MessengerReference), options.MessengerReference, null)
			};
			return messenger;
		});
		configurator.Service.TryAddSingleton<InMemoryDispatcher>();
		configurator.Service.AddTransient<IRecipientRegistrar, InMemoryRecipientRegistrar>();
		configurator.SerFactory<InMemoryBusFactory>();
	}
}