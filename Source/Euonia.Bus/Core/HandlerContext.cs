using System.Collections.Concurrent;
using System.Linq.Expressions;
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

	private readonly ConcurrentDictionary<string, List<MessageHandlerFactory>> _handlerContainer = new();
	private readonly IServiceProvider _provider;
	private readonly ILogger<HandlerContext> _logger;

	/// <summary>
	/// Initialize a new instance of <see cref="HandlerContext"/>
	/// </summary>
	/// <param name="provider"></param>
	public HandlerContext(IServiceProvider provider)
	{
		_provider = provider;
		_logger = provider.GetService<ILoggerFactory>()?.CreateLogger<HandlerContext>();
	}

	#region Handling register

	internal virtual void Register<TMessage, THandler>()
		where TMessage : class
		where THandler : IHandler<TMessage>
	{
		var channel = MessageCache.Default.GetOrAddChannel<TMessage>();

		MessageHandler Handling(IServiceProvider provider)
		{
			var handler = provider.GetService<THandler>();
			return (message, context, token) => handler.HandleAsync((TMessage)message, context, token);
		}

		ConcurrentDictionarySafeRegister(channel, Handling, _handlerContainer);
		MessageSubscribed?.Invoke(this, new MessageSubscribedEventArgs(channel, typeof(TMessage), typeof(THandler)));
	}

	internal void Register(MessageRegistration registration)
	{
		MessageHandler Handling(IServiceProvider provider)
		{
			var handler = ActivatorUtilities.GetServiceOrCreateInstance(provider, registration.HandlerType);

			return (message, context, token) =>
			{
				var arguments = GetArguments(registration.Method, message, context, token);
				var expression = Expression.Call(Expression.Constant(handler), registration.Method, arguments);

				return Expression.Lambda<Func<Task>>(expression).Compile()();
			};
		}

		ConcurrentDictionarySafeRegister(registration.Channel, Handling, _handlerContainer);
		MessageSubscribed?.Invoke(this, new MessageSubscribedEventArgs(registration.Channel, registration.MessageType, registration.HandlerType));
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

		var name = MessageCache.Default.GetOrAddChannel(message.GetType());
		await HandleAsync(name, message, context, cancellationToken);
	}

	/// <inheritdoc />
	public virtual async Task HandleAsync(string channel, object message, MessageContext context, CancellationToken cancellationToken = default)
	{
		if (message == null)
		{
			return;
		}

		var tasks = new List<Task>();
		using var scope = _provider.GetRequiredService<IServiceScopeFactory>().CreateScope();

		if (!_handlerContainer.TryGetValue(channel, out var handling))
		{
			throw new InvalidOperationException("No handler registered for message");
		}

		// Get handler instance from service provider using Expression Tree

		foreach (var factory in handling)
		{
			var handler = factory(scope.ServiceProvider);
			tasks.Add(handler(message, context, cancellationToken));
			// var handler = ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, handlerType);
			//
			// var arguments = GetMethodArguments(handleMethod, message, context, cancellationToken);
			// if (arguments == null)
			// {
			// 	_logger.LogWarning("Method '{Name}' parameter number not matches", handleMethod.Name);
			// }
			// else
			// {
			// 	tasks.Add(Invoke(handleMethod, handler, arguments));
			// }
		}

		if (tasks.Count == 0)
		{
			return;
		}

		await Task.WhenAll(tasks).ContinueWith(_ =>
		{
			_logger?.LogInformation("Message {Id} was completed handled", context.MessageId);
		}, cancellationToken);

		context.Dispose();
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

	private static Expression[] GetArguments(MethodBase method, object message, MessageContext context, CancellationToken cancellationToken)
	{
		var parameterInfos = method.GetParameters();
		var arguments = new Expression[parameterInfos.Length];
		switch (parameterInfos.Length)
		{
			case 0:
				break;
			case 1:
			{
				var parameterType = parameterInfos[0].ParameterType;

				if (parameterType == typeof(MessageContext))
				{
					arguments[0] = Expression.Constant(context);
				}
				else if (parameterType == typeof(CancellationToken))
				{
					arguments[0] = Expression.Constant(cancellationToken);
				}
				else
				{
					arguments[0] = Expression.Constant(message);
				}
			}
				break;
			case 2:
			case 3:
			{
				arguments[0] ??= Expression.Constant(message);

				for (var index = 1; index < parameterInfos.Length; index++)
				{
					if (parameterInfos[index].ParameterType == typeof(MessageContext))
					{
						arguments[index] = Expression.Constant(context);
					}

					if (parameterInfos[index].ParameterType == typeof(CancellationToken))
					{
						arguments[index] = Expression.Constant(cancellationToken);
					}
				}
			}
				break;
			default:
				return null;
		}

		return arguments;
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