using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable MemberCanBePrivate.Global

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The message bus configurator.
/// </summary>
public class BusConfigurator : IBusConfigurator
{
	private readonly List<MessageRegistration> _registrations = [];

	internal MessageConventionBuilder ConventionBuilder { get; } = new();

	internal ConcurrentDictionary<Type, TransportStrategyBuilder> StrategyBuilders { get; } = new();

	internal Func<IServiceProvider, IIdentityProvider> IdentityProviderFactory { get; set; } = null!;

	/// <summary>
	/// Gets the message handle registrations.
	/// </summary>
	public IReadOnlyList<MessageRegistration> Registrations => _registrations;
	
	/// <summary>
	/// Gets the transports with configured strategies.
	/// </summary>
	public IReadOnlyList<Type> StrategyAssignedTypes => StrategyBuilders.Keys.ToList();

	/// <summary>
	/// Register the message handlers.
	/// </summary>
	/// <param name="assemblies"></param>
	/// <returns></returns>
	public IBusConfigurator RegisterHandlers(params Assembly[] assemblies)
	{
		return RegisterHandlers(() => assemblies.SelectMany(assembly => assembly.DefinedTypes));
	}

	/// <summary>
	/// Register the message handlers.
	/// </summary>
	/// <param name="typesFactory"></param>
	/// <returns></returns>
	public IBusConfigurator RegisterHandlers(Func<IEnumerable<Type>> typesFactory)
	{
		return RegisterHandlers(typesFactory());
	}

	/// <summary>
	/// Register the message handlers.
	/// </summary>
	/// <param name="types"></param>
	/// <returns></returns>
	public IBusConfigurator RegisterHandlers(IEnumerable<Type> types)
	{
		var registrations = MessageHandlerFinder.Find(types).ToList();
		_registrations.AddRange(registrations);
		return this;
	}

	/// <summary>
	/// Sets the message convention.
	/// </summary>
	/// <param name="configure"></param>
	/// <returns></returns>
	public IBusConfigurator SetConventions(Action<MessageConventionBuilder> configure)
	{
		configure?.Invoke(ConventionBuilder);
		return this;
	}

	/// <summary>
	/// Sets the message handling strategies.
	/// </summary>
	/// <param name="transport"></param>
	/// <param name="configure"></param>
	/// <returns></returns>
	public IBusConfigurator SetStrategies(Type transport, Action<TransportStrategyBuilder> configure)
	{
		if (configure != null)
		{
			var builder = StrategyBuilders.GetOrAdd(transport, _ => new TransportStrategyBuilder());
			configure(builder);
		}

		{
		}
		return this;
	}

	/// <summary>
	/// Register the message identity provider.
	/// </summary>
	/// <param name="accessor"></param>
	/// <returns></returns>
	public IBusConfigurator SetIdentityProvider(IdentityAccessor accessor)
	{
		IdentityProviderFactory = _ => new DefaultIdentityProvider(accessor);
		return this;
	}

	/// <summary>
	/// Register the message identity provider.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public IBusConfigurator SetIdentityProvider<T>()
		where T : class, IIdentityProvider
	{
		IdentityProviderFactory = ActivatorUtilities.GetServiceOrCreateInstance<T>;
		return this;
	}
}