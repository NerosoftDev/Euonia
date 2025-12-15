using System.Reflection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The bus configurator abstract interface.
/// </summary>`
public interface IBusConfigurator
{
	/// <summary>
	/// Get the message handle registrations.
	/// </summary>
	IReadOnlyList<MessageRegistration> Registrations { get; }

	/// <summary>
	/// Gets the transports with configured strategies.
	/// </summary>
	IReadOnlyList<Type> StrategyAssignedTypes { get; }

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

	/// <summary>
	/// Sets the message convention.
	/// </summary>
	/// <param name="configure"></param>
	/// <returns></returns>
	IBusConfigurator SetConventions(Action<MessageConventionBuilder> configure);

	/// <summary>
	/// Sets the message handling strategies.
	/// </summary>
	/// <param name="transport"></param>
	/// <param name="configure"></param>
	/// <returns></returns>
	IBusConfigurator SetStrategies(Type transport, Action<TransportStrategyBuilder> configure);
}