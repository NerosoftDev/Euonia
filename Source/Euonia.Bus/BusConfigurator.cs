using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable MemberCanBePrivate.Global

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Configures the message bus by registering handlers, setting conventions,
/// assigning transport strategies and configuring identity providers.
/// </summary>
public class BusConfigurator : IBusConfigurator
{
	/// <summary>
	/// Holds discovered message handler registrations.
	/// </summary>
	private readonly List<MessageRegistration> _registrations = [];

	/// <summary>
	/// Builder used to configure message naming and discovery conventions.
	/// </summary>
	internal MessageConventionBuilder ConventionBuilder { get; } = new();

	/// <summary>
	/// Builders for transport-specific strategies keyed by transport type.
	/// </summary>
	internal ConcurrentDictionary<Type, TransportStrategyBuilder> StrategyBuilders { get; } = new();

	/// <summary>
	/// Factory that resolves or creates an <see cref="IIdentityProvider"/> from an <see cref="IServiceProvider"/>.
	/// </summary>
	internal Func<IServiceProvider, IIdentityProvider> IdentityProviderFactory { get; set; } = null!;

	/// <summary>
	/// Read-only list of registered message handler registrations.
	/// </summary>
	public IReadOnlyList<MessageRegistration> Registrations => _registrations;

	/// <summary>
	/// Types for which a transport strategy has been configured.
	/// </summary>
	public IReadOnlyList<Type> StrategyAssignedTypes => StrategyBuilders.Keys.ToList();

	/// <summary>
	/// Scans the provided assemblies for handler types and registers them.
	/// </summary>
	/// <param name="assemblies">Assemblies to scan for message handlers.</param>
	/// <returns>The current <see cref="IBusConfigurator"/> for fluent configuration.</returns>
	public IBusConfigurator RegisterHandlers(params Assembly[] assemblies)
	{
		return RegisterHandlers(() => assemblies.SelectMany(assembly => assembly.DefinedTypes));
	}

	/// <summary>
	/// Registers handler types returned from a factory function.
	/// </summary>
	/// <param name="typesFactory">Factory that provides handler types to register.</param>
	/// <returns>The current <see cref="IBusConfigurator"/> for fluent configuration.</returns>
	public IBusConfigurator RegisterHandlers(Func<IEnumerable<Type>> typesFactory)
	{
		return RegisterHandlers(typesFactory());
	}

	/// <summary>
	/// Registers the specified handler types with the bus configuration.
	/// </summary>
	/// <param name="types">Collection of handler types to register.</param>
	/// <returns>The current <see cref="IBusConfigurator"/> for fluent configuration.</returns>
	public IBusConfigurator RegisterHandlers(IEnumerable<Type> types)
	{
		var registrations = MessageHandlerFinder.Find(types).ToList();
		_registrations.AddRange(registrations);
		return this;
	}

	/// <summary>
	/// Applies message conventions using the provided configuration action.
	/// </summary>
	/// <param name="configure">Action that configures the <see cref="MessageConventionBuilder"/>.</param>
	/// <returns>The current <see cref="IBusConfigurator"/> for fluent configuration.</returns>
	public IBusConfigurator SetConventions(Action<MessageConventionBuilder> configure)
	{
		configure?.Invoke(ConventionBuilder);
		return this;
	}

	/// <summary>
	/// Configures a transport strategy for a given transport type.
	/// </summary>
	/// <param name="transport">Transport type to configure.</param>
	/// <param name="configure">Action that configures the <see cref="TransportStrategyBuilder"/> for the transport.</param>
	/// <returns>The current <see cref="IBusConfigurator"/> for fluent configuration.</returns>
	public IBusConfigurator SetStrategy(Type transport, Action<TransportStrategyBuilder> configure)
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
	/// Sets a concrete identity accessor instance to be wrapped by the default identity provider.
	/// </summary>
	/// <param name="accessor">Identity accessor that provides caller identity information.</param>
	/// <returns>The current <see cref="IBusConfigurator"/> for fluent configuration.</returns>
	public IBusConfigurator SetIdentityProvider(IdentityAccessor accessor)
	{
		IdentityProviderFactory = _ => new DefaultIdentityProvider(accessor);
		return this;
	}

	/// <summary>
	/// Configures the identity provider by specifying an implementation type that implements <see cref="IIdentityProvider"/>.
	/// The provider will be resolved from the <see cref="IServiceProvider"/> or created if missing.
	/// </summary>
	/// <typeparam name="T">Identity provider implementation type.</typeparam>
	/// <returns>The current <see cref="IBusConfigurator"/> for fluent configuration.</returns>
	public IBusConfigurator SetIdentityProvider<T>()
		where T : class, IIdentityProvider
	{
		IdentityProviderFactory = ActivatorUtilities.GetServiceOrCreateInstance<T>;
		return this;
	}
}