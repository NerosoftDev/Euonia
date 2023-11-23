using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Default message handler context using Microsoft dependency injection.
/// </summary>
public class HandlerContext : IHandlerContext
{
	/// <summary>
	/// 
	/// </summary>
	public event EventHandler<MessageSubscribedEventArgs> MessageSubscribed;

	private readonly ConcurrentDictionary<string, List<(Type, MethodInfo)>> _handlerContainer = new();
	private static readonly ConcurrentDictionary<string, Type> _messageTypeMapping = new();
	private readonly IServiceProvider _provider;
	private readonly MessageConvert _convert;
	private readonly ILogger<HandlerContext> _logger;

	/// <summary>
	/// Initialize a new instance of <see cref="HandlerContext"/>
	/// </summary>
	/// <param name="provider"></param>
	/// <param name="convert"></param>
	public HandlerContext(IServiceProvider provider, MessageConvert convert)
	{
		_provider = provider;
		_convert = convert;
		_logger = provider.GetService<ILoggerFactory>()?.CreateLogger<HandlerContext>();
	}

	#region Handler register

	internal virtual void Register<TMessage, THandler>()
		where TMessage : class
		where THandler : IHandler<TMessage>
	{
		Register(typeof(TMessage), typeof(THandler), typeof(THandler).GetMethod(nameof(IHandler<TMessage>.HandleAsync), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
	}

	internal virtual void Register(Type messageType, Type handlerType, MethodInfo method)
	{
		var messageName = messageType.FullName;

		_messageTypeMapping.GetOrAdd(messageName, messageType);
		ConcurrentDictionarySafeRegister(messageName, (handlerType, method), _handlerContainer);
		MessageSubscribed?.Invoke(this, new MessageSubscribedEventArgs(messageType, handlerType));
	}

	internal void Register(string messageName, Type handlerType, MethodInfo method)
	{
		ConcurrentDictionarySafeRegister(messageName, (handlerType, method), _handlerContainer);
		MessageSubscribed?.Invoke(this, new MessageSubscribedEventArgs(messageName, method.DeclaringType));
	}

	#endregion

	#region Handle message

	/// <inheritdoc />
	public virtual async Task HandleAsync(object message, MessageContext context, CancellationToken cancellationToken = default)
	{
		if (message == null)
		{
			return;
		}

		var name = message.GetType().FullName;
		await HandleAsync(name, message, context, cancellationToken);
	}

	/// <inheritdoc />
	public virtual async Task HandleAsync(string name, object message, MessageContext context, CancellationToken cancellationToken = default)
	{
		if (message == null)
		{
			return;
		}

		var tasks = new List<Task>();
		using var scope = _provider.GetRequiredService<IServiceScopeFactory>().CreateScope();

		if (!_handlerContainer.TryGetValue(name, out var handlerTypes))
		{
			throw new InvalidOperationException("No handler registered for message");
		}

		foreach (var (handlerType, handleMethod) in handlerTypes)
		{
			var handler = ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, handlerType);

			var parameters = GetMethodArguments(handleMethod, context, context, cancellationToken);
			if (parameters == null)
			{
				_logger.LogWarning("Method '{Name}' parameter number not matches", handleMethod.Name);
			}
			else
			{
				tasks.Add(Invoke(handleMethod, handler, parameters));
			}
		}

		if (tasks.Count == 0)
		{
			return;
		}

		await Task.WhenAll(tasks).ContinueWith(_ =>
		{
			_logger?.LogInformation("Message {Id} was completed handled", context.MessageId);
		}, cancellationToken);
	}

	#endregion

	#region Supports

	private void ConcurrentDictionarySafeRegister<TKey, TValue>(TKey key, TValue value, ConcurrentDictionary<TKey, List<TValue>> registry)
	{
		lock (_handlerContainer)
		{
			if (registry.TryGetValue(key, out var handlers))
			{
				if (handlers != null)
				{
					if (!handlers.Contains(value))
					{
						registry[key].Add(value);
					}
				}
				else
				{
					registry[key] = new List<TValue> { value };
				}
			}
			else
			{
				registry.TryAdd(key, new List<TValue> { value });
			}
		}
	}

	private object[] GetMethodArguments(MethodBase method, object message, MessageContext context, CancellationToken cancellationToken)
	{
		var parameterInfos = method.GetParameters();
		var parameters = new object[parameterInfos.Length];
		switch (parameterInfos.Length)
		{
			case 0:
				break;
			case 1:
			{
				var parameterType = parameterInfos[0].ParameterType;

				if (parameterType == typeof(MessageContext))
				{
					parameters[0] = context;
				}
				else if (parameterType == typeof(CancellationToken))
				{
					parameters[0] = cancellationToken;
				}
				else
				{
					parameters[0] = _convert(message, parameterType);
				}
			}
				break;
			case 2:
			case 3:
			{
				for (var index = 0; index < parameterInfos.Length; index++)
				{
					if (parameterInfos[index].ParameterType == typeof(MessageContext))
					{
						parameters[index] = context;
					}

					if (parameterInfos[index].ParameterType == typeof(CancellationToken))
					{
						parameters[index] = cancellationToken;
					}
				}

				parameters[0] ??= _convert(message, parameterInfos[0].ParameterType);
			}
				break;
			default:
				return null;
		}

		return parameters;
	}

	private static Task Invoke(MethodInfo method, object handler, params object[] parameters)
	{
		if (method.ReturnType.IsAssignableTo(typeof(IAsyncResult)))
		{
			return (Task)method.Invoke(handler, parameters);
		}
		else
		{
			return Task.Run(() => method.Invoke(handler, parameters));
		}
	}

	#endregion
}