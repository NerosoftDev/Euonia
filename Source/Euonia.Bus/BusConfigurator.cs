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

	private MessageConventionBuilder ConventionBuilder { get; } = new();

	/// <summary>
	/// The message handler types.
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
	/// 
	/// </summary>
	/// <typeparam name="TFactory"></typeparam>
	/// <returns></returns>
	public IBusConfigurator SerFactory<TFactory>()
		where TFactory : class, IBusFactory
	{
		Service.AddSingleton<IBusFactory, TFactory>();
		return this;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="factory"></param>
	/// <typeparam name="TFactory"></typeparam>
	/// <returns></returns>
	public IBusConfigurator SerFactory<TFactory>(TFactory factory)
		where TFactory : class, IBusFactory
	{
		Service.AddSingleton<IBusFactory>(factory);
		return this;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="factory"></param>
	/// <typeparam name="TFactory"></typeparam>
	/// <returns></returns>
	public IBusConfigurator SerFactory<TFactory>(Func<IServiceProvider, TFactory> factory)
		where TFactory : class, IBusFactory
	{
		Service.TryAddSingleton<IBusFactory>(factory);
		return this;
	}

	/// <summary>
	/// Set the message serializer.
	/// </summary>
	/// <typeparam name="TSerializer"></typeparam>
	/// <returns></returns>
	public BusConfigurator SetSerializer<TSerializer>()
		where TSerializer : class, IMessageSerializer
	{
		Service.TryAddSingleton<IMessageSerializer, TSerializer>();
		return this;
	}

	/// <summary>
	/// Set the message serializer.
	/// </summary>
	/// <param name="serializer"></param>
	/// <typeparam name="TSerializer"></typeparam>
	/// <returns></returns>
	public BusConfigurator SetSerializer<TSerializer>(TSerializer serializer)
		where TSerializer : class, IMessageSerializer
	{
		Service.TryAddSingleton<IMessageSerializer>(serializer);
		return this;
	}

	/// <summary>
	/// Set the message store provider.
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
	/// Set the message store provider.
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
	public BusConfigurator RegisterHandlers(params Assembly[] assemblies)
	{
		return RegisterHandlers(() => assemblies.SelectMany(assembly => assembly.DefinedTypes));
	}

	/// <summary>
	/// Register the message handlers.
	/// </summary>
	/// <param name="typesFactory"></param>
	/// <returns></returns>
	public BusConfigurator RegisterHandlers(Func<IEnumerable<Type>> typesFactory)
	{
		return RegisterHandlers(typesFactory());
	}

	/// <summary>
	/// Register the message handlers.
	/// </summary>
	/// <param name="types"></param>
	/// <returns></returns>
	public BusConfigurator RegisterHandlers(IEnumerable<Type> types)
	{
		var registrations = MessageHandlerFinder.Find(types);
		var handlerTypes = registrations.Select(x => x.HandlerType).Distinct();
		foreach (var handlerType in handlerTypes)
		{
			Service.TryAddScoped(handlerType);
		}
		_registrations.AddRange(registrations);
		return this;
	}

	/// <summary>
	/// Set the message convention.
	/// </summary>
	/// <param name="configure"></param>
	/// <returns></returns>
	public BusConfigurator SetConventions(Action<MessageConventionBuilder> configure)
	{
		configure?.Invoke(ConventionBuilder);
		Service.TryAddSingleton<IMessageConvention>(ConventionBuilder.Convention);
		return this;
	}
}