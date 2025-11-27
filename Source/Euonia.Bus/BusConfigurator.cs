using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable MemberCanBePrivate.Global

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The message bus configurator.
/// </summary>
public class BusConfigurator : IBusConfigurator
{
	private readonly List<MessageRegistration> _registrations = new();

	internal MessageConventionBuilder ConventionBuilder { get; } = new();

	/// <summary>
	/// Gets the message handle registrations.
	/// </summary>
	public IReadOnlyList<MessageRegistration> Registrations => _registrations;

	/// <summary>
	/// Initialize a new instance of <see cref="BusConfigurator"/>
	/// </summary>
	/// <param name="service"></param>
	public BusConfigurator(IServiceCollection service)
	{
		Service = service;
	}

	/// <inheritdoc />
	public IServiceCollection Service { get; }

	/// <summary>
	/// Sets the bus factory.
	/// </summary>
	/// <typeparam name="TFactory"></typeparam>
	/// <returns></returns>
	public IBusConfigurator SetFactory<TFactory>()
		where TFactory : class, IBusFactory
	{
		Service.TryAddSingleton<IBusFactory, TFactory>();
		return this;
	}

	/// <summary>
	/// Sets the bus factory.
	/// </summary>
	/// <param name="factory"></param>
	/// <typeparam name="TFactory"></typeparam>
	/// <returns></returns>
	public IBusConfigurator SetFactory<TFactory>(TFactory factory)
		where TFactory : class, IBusFactory
	{
		Service.TryAddSingleton<IBusFactory>(factory);
		return this;
	}

	/// <summary>
	/// Sets the message serializer.
	/// </summary>
	/// <param name="factory"></param>
	/// <typeparam name="TFactory"></typeparam>
	/// <returns></returns>
	public IBusConfigurator SetFactory<TFactory>(Func<IServiceProvider, TFactory> factory)
		where TFactory : class, IBusFactory
	{
		Service.TryAddSingleton<IBusFactory>(factory);
		return this;
	}

	/// <summary>
	/// Sets the message serializer.
	/// </summary>
	/// <typeparam name="TSerializer"></typeparam>
	/// <returns></returns>
	public IBusConfigurator SetSerializer<TSerializer>()
		where TSerializer : class, IMessageSerializer
	{
		Service.TryAddSingleton<IMessageSerializer, TSerializer>();
		return this;
	}

	/// <summary>
	/// Sets the message serializer.
	/// </summary>
	/// <param name="serializer"></param>
	/// <typeparam name="TSerializer"></typeparam>
	/// <returns></returns>
	public IBusConfigurator SetSerializer<TSerializer>(TSerializer serializer)
		where TSerializer : class, IMessageSerializer
	{
		Service.TryAddSingleton<IMessageSerializer>(serializer);
		return this;
	}

	/// <summary>
	/// Sets the message store provider.
	/// </summary>
	/// <typeparam name="TStore"></typeparam>
	/// <returns></returns>
	public IBusConfigurator SetMessageStore<TStore>()
		where TStore : class, IMessageStore
	{
		Service.TryAddTransient<IMessageStore, TStore>();
		return this;
	}

	/// <summary>
	/// Sets the message store provider.
	/// </summary>
	/// <param name="store"></param>
	/// <typeparam name="TStore"></typeparam>
	/// <returns></returns>
	public IBusConfigurator SetMessageStore<TStore>(Func<IServiceProvider, TStore> store)
		where TStore : class, IMessageStore
	{
		Service.TryAddTransient<IMessageStore>(store);
		return this;
	}

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
		
		var handlerTypes = registrations.Select(x => x.HandlerType)
		                                .Distinct()
		                                .ToList();
		foreach (var handlerType in handlerTypes)
		{
			Service.TryAddScoped(handlerType);
		}

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
	/// Register the message identity provider.
	/// </summary>
	/// <param name="accessor"></param>
	/// <returns></returns>
	public IBusConfigurator SetIdentityProvider(IdentityAccessor accessor)
	{
		Service.TryAddSingleton<IIdentityProvider>(_ => new DefaultIdentityProvider(accessor));
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
		Service.TryAddSingleton<IIdentityProvider, T>();
		return this;
	}
}