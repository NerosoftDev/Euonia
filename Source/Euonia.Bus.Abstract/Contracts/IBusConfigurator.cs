using System.Reflection;
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
	/// Get the message handle registrations.
	/// </summary>
	IReadOnlyList<MessageRegistration> Registrations { get; }

	/// <summary>
	/// Set the service bus factory.
	/// </summary>
	/// <typeparam name="TFactory"></typeparam>
	/// <returns></returns>
	IBusConfigurator SetFactory<TFactory>()
		where TFactory : class, IBusFactory;

	/// <summary>
	/// Set the service bus factory.
	/// </summary>
	/// <param name="factory"></param>
	/// <typeparam name="TFactory"></typeparam>
	/// <returns></returns>
	IBusConfigurator SetFactory<TFactory>(TFactory factory)
		where TFactory : class, IBusFactory;

	/// <summary>
	/// Set the service bus factory.
	/// </summary>
	/// <param name="factory"></param>
	/// <typeparam name="TFactory"></typeparam>
	/// <returns></returns>
	IBusConfigurator SetFactory<TFactory>(Func<IServiceProvider, TFactory> factory)
		where TFactory : class, IBusFactory;

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

	/// <summary>
    /// Register the message handlers.
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
	IBusConfigurator RegisterHandlers(params Assembly[] assemblies);

	/// <summary>
    /// Register the message handlers.
    /// </summary>
    /// <param name="typesFactory"></param>
    /// <returns></returns>
	IBusConfigurator RegisterHandlers(Func<IEnumerable<Type>> typesFactory);

	/// <summary>
    /// Register the message handlers.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
	IBusConfigurator RegisterHandlers(IEnumerable<Type> types);

	/// <summary>
	/// Register the message identity provider.
	/// </summary>
	/// <param name="accessor"></param>
	/// <returns></returns>
	IBusConfigurator SetIdentityProvider(IdentityAccessor accessor);

	/// <summary>
	/// Register the message identity provider.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	IBusConfigurator SetIdentityProvider<T>()
		where T : class, IIdentityProvider;
}