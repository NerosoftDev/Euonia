using System.Reflection;

namespace Nerosoft.Euonia.Bus;

internal class MessageHandlerFinder
{
	private const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

	public static IEnumerable<MessageRegistration> Find(IEnumerable<Type> types)
	{
		return types.SelectMany(Resolve);
	}

	public static IEnumerable<MessageRegistration> Find(params Assembly[] assemblies)
	{
		var types = assemblies.SelectMany(x => x.DefinedTypes);

		return Find(types);
	}

	public static IEnumerable<MessageRegistration> Find(params Type[] types)
	{
		return Find(types.AsEnumerable());
	}

	/// <summary>
	/// Extract message registrations from the specified handler type.
	/// </summary>
	/// <param name="handlerType"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	private static IEnumerable<MessageRegistration> Resolve(Type handlerType)
	{
		if (!handlerType.IsClass || handlerType.IsInterface || handlerType.IsAbstract)
		{
			return Enumerable.Empty<MessageRegistration>();
		}

		var registrations = new List<MessageRegistration>();

		var methods = handlerType.GetMethods(BINDING_FLAGS)
		                         .Where(method => method.HasAttribute<SubscribeAttribute>())
		                         .ToList();

		if (methods.Count > 0)
		{
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
					var channel = attribute.Name.DefaultIfNullOrWhiteSpace(MessageCache.Default.GetOrAddChannel(firstParameter.ParameterType));
					var registration = new MessageRegistration(channel, firstParameter.ParameterType, handlerType, method);
					registrations.Add(registration);
				}
			}
		}

		// Extract IHandler<> interfaces from the handler type
		var interfaces = handlerType.GetInterfaces()
		                            .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IHandler<,>))
		                            .ToList();
		if (interfaces.Count > 0)
		{
			foreach (var @interface in interfaces)
			{
				var messageType = @interface.GetGenericArguments()[0];
				var method = handlerType.GetMethod(nameof(IHandler<,>.HandleAsync), BINDING_FLAGS, null, [messageType, typeof(MessageContext), typeof(CancellationToken)], null);
				var registration = new MessageRegistration(MessageCache.Default.GetOrAddChannel(messageType), messageType, handlerType, method);
				registrations.Add(registration);
			}
		}

		{
		}

		return registrations;
	}
}