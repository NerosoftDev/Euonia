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
	private const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

	private MessageConventionBuilder ConventionBuilder { get; } = new();

	/// <summary>
	/// The message handler types.
	/// </summary>
	internal List<MessageRegistration> Registrations { get; } = new();

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

	/// <inheritdoc />
	public IEnumerable<string> GetSubscriptions()
	{
		return Registrations.Select(t => t.Channel);
	}

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
	/// <param name="assembly"></param>
	/// <returns></returns>
	public BusConfigurator RegisterHandlers(Assembly assembly)
	{
		return RegisterHandlers(() => assembly.DefinedTypes);
	}

	/// <summary>
	/// Register the message handlers.
	/// </summary>
	/// <param name="handlerTypesFactory"></param>
	/// <returns></returns>
	public BusConfigurator RegisterHandlers(Func<IEnumerable<Type>> handlerTypesFactory)
	{
		return RegisterHandlers(handlerTypesFactory());
	}

	/// <summary>
	/// Register the message handlers.
	/// </summary>
	/// <param name="handlerTypes"></param>
	/// <returns></returns>
	public BusConfigurator RegisterHandlers(IEnumerable<Type> handlerTypes)
	{
		foreach (var handlerType in handlerTypes)
		{
			if (!handlerType.IsClass || handlerType.IsInterface || handlerType.IsAbstract)
			{
				continue;
			}

			if (handlerType.IsImplementsGeneric(typeof(IHandler<>)))
			{
				var interfaces = handlerType.GetInterfaces().Where(t => t.IsGenericType);

				foreach (var @interface in interfaces)
				{
					var messageType = @interface.GetGenericArguments().FirstOrDefault();

					if (messageType == null)
					{
						continue;
					}

					var method = @interface.GetMethod(nameof(IHandler<object>.HandleAsync), BINDING_FLAGS, null, new[] { messageType, typeof(MessageContext), typeof(CancellationToken) }, null);

					var registration = new MessageRegistration(MessageCache.Default.GetOrAddChannel(messageType), messageType, handlerType, method);

					Registrations.Add(registration);

					Service.TryAddScoped(typeof(IHandler<>).MakeGenericType(messageType), handlerType);
				}

				Service.TryAddScoped(handlerType);
			}
			else
			{
				var methods = handlerType.GetMethods(BINDING_FLAGS).Where(method => method.GetCustomAttributes<SubscribeAttribute>().Any());

				if (!methods.Any())
				{
					continue;
				}

				foreach (var method in methods)
				{
					var parameters = method.GetParameters();

					if (!parameters.Any(t => t.ParameterType != typeof(CancellationToken) && t.ParameterType != typeof(MessageContext)))
					{
						throw new InvalidOperationException("Invalid handler method.");
					}

					var firstParameter = parameters[0];

					if (firstParameter.ParameterType.IsPrimitiveType())
					{
						throw new InvalidOperationException("The first parameter of handler method must be message type");
					}

					switch (parameters.Length)
					{
						case 2 when parameters[1].ParameterType != typeof(MessageContext) || parameters[1].ParameterType != typeof(CancellationToken):
							throw new InvalidOperationException("The second parameter of handler method must be MessageContext or CancellationToken if the method contains 2 parameters");
						case 3 when parameters[1].ParameterType != typeof(MessageContext) && parameters[2].ParameterType != typeof(CancellationToken):
							throw new InvalidOperationException("The second and third parameter of handler method must be MessageContext and CancellationToken if the method contains 3 parameters");
					}

					var attributes = method.GetCustomAttributes<SubscribeAttribute>();

					foreach (var attribute in attributes)
					{
						var registration = new MessageRegistration(attribute.Name, firstParameter.ParameterType, handlerType, method);
						Registrations.Add(registration);
					}
				}

				Service.TryAddScoped(handlerType);
			}
		}

		return this;

		void ValidateMessageType(Type messageType)
		{
			if (messageType.IsPrimitiveType())
			{
				throw new InvalidOperationException("The message type cannot be a primitive type.");
			}

			if (messageType.IsClass)
			{
				throw new InvalidOperationException("The message type must be a class.");
			}

			if (messageType.IsAbstract)
			{
				throw new InvalidOperationException("The message type cannot be an abstract class.");
			}

			if (messageType.IsInterface)
			{
				throw new InvalidOperationException("The message type cannot be an interface.");
			}
		}
	}

	/// <summary>
	/// Set the message convention.
	/// </summary>
	/// <param name="configure"></param>
	/// <returns></returns>
	public BusConfigurator SetConventions(Action<MessageConventionBuilder> configure)
	{
		configure?.Invoke(ConventionBuilder);
		Service.TryAddSingleton(ConventionBuilder.Convention);
		return this;
	}
}