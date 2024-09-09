using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The bus configurator abstract interface.
/// </summary>`
public interface IBusConfigurator
{
	/// <summary>
	/// Get the service collection.
	/// </summary>
	IServiceCollection Service { get; }

	/// <summary>
	/// Set the message store provider.
	/// </summary>
	/// <typeparam name="TStore"></typeparam>
	/// <returns></returns>
	IBusConfigurator SetMessageStore<TStore>()
		where TStore : class, IMessageStore;

	/// <summary>
	/// Set the message store provider.
	/// </summary>
	/// <param name="store"></param>
	/// <typeparam name="TStore"></typeparam>
	/// <returns></returns>
	IBusConfigurator SetMessageStore<TStore>(Func<IServiceProvider, TStore> store)
		where TStore : class, IMessageStore;
}