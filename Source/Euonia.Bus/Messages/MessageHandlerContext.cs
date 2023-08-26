using System.Collections.Concurrent;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Default message handler context using Microsoft dependency injection.
/// </summary>
public class MessageHandlerContext : IMessageHandlerContext
{
    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<MessageSubscribedEventArgs> MessageSubscribed;

    private readonly ConcurrentDictionary<string, List<Type>> _handlerContainer = new();
    private static readonly ConcurrentDictionary<string, Type> _messageTypeMapping = new();
    private readonly IServiceProvider _provider;
    private readonly MessageConversionDelegate _conversion;
    private readonly ILogger<MessageHandlerContext> _logger;

    /// <summary>
    /// Initialize a new instance of <see cref="MessageHandlerContext"/>
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="conversion"></param>
    public MessageHandlerContext(IServiceProvider provider, MessageConversionDelegate conversion)
    {
        _provider = provider;
        _conversion = conversion;
        _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<MessageHandlerContext>();
    }

    /// <inheritdoc />
    public virtual IMediator GetMediator()
    {
        return _provider.GetService<IMediator>();
    }

    #region Handler register

    /// <inheritdoc />
    public virtual void Register<TMessage, THandler>()
        where TMessage : IMessage
        where THandler : IMessageHandler<TMessage>
    {
        if (IsHandlerRegistered<TMessage, THandler>())
        {
            return;
        }

        Register(typeof(TMessage), typeof(THandler));
    }

    /// <inheritdoc />
    public virtual void Register(Type messageType, Type handlerType)
    {
        if (IsHandlerRegistered(messageType, handlerType))
        {
            return;
        }

        var messageName = messageType.FullName;

        _messageTypeMapping.GetOrAdd(messageName, messageType);
        ConcurrentDictionarySafeRegister(messageName, handlerType, _handlerContainer);
        MessageSubscribed?.Invoke(this, new MessageSubscribedEventArgs(messageType, handlerType));
    }

    /// <inheritdoc />
    public void Register(string messageName, Type handlerType)
    {
        if (IsHandlerRegistered(messageName, handlerType))
        {
            return;
        }

        ConcurrentDictionarySafeRegister(messageName, handlerType, _handlerContainer);
        MessageSubscribed?.Invoke(this, new MessageSubscribedEventArgs(messageName, handlerType));
    }

    /// <inheritdoc />
    public virtual bool IsHandlerRegistered<TMessage, THandler>()
        where TMessage : IMessage
        where THandler : IMessageHandler<TMessage>
    {
        return IsHandlerRegistered(typeof(TMessage), typeof(THandler));
    }

    /// <inheritdoc />
    public virtual bool IsHandlerRegistered(Type messageType, Type handlerType)
    {
        return IsHandlerRegistered(messageType.FullName!, handlerType);
    }

    /// <inheritdoc />
    public virtual bool IsHandlerRegistered(string messageName, Type handlerType)
    {
        if (_handlerContainer.TryGetValue(messageName, out var handlers))
        {
            return handlers != null && handlers.Contains(handlerType);
        }

        return false;
    }

    #endregion

    #region Handle message

    /// <inheritdoc />
    public virtual async Task HandleAsync(IMessage message, MessageContext context, CancellationToken cancellationToken = default)
    {
        if (message == null)
        {
            return;
        }

        var tasks = new List<Task>();
        using var scope = _provider.GetRequiredService<IServiceScopeFactory>().CreateScope();

        if (message is INamedMessage namedMessage)
        {
            if (!_handlerContainer.TryGetValue(namedMessage.Name, out var handlerTypes))
            {
                return;
            }

            foreach (var handlerType in handlerTypes)
            {
                if (handlerType.IsSubclassOf(typeof(IMessageHandler)))
                {
                    if (!_messageTypeMapping.TryGetValue(namedMessage.Name, out var messageType) || !messageType.IsAssignableTo(typeof(IMessage)))
                    {
                        continue;
                    }

                    var messageToHandle = (IMessage)_conversion(namedMessage.Data, messageType);

                    var handler = (IMessageHandler)ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, handlerType);

                    tasks.Add(handler.HandleAsync(messageToHandle, context, cancellationToken));
                }
                else
                {
                    var methods = handlerType.GetRuntimeMethods().Where(method => method.GetCustomAttributes<EventSubscribeAttribute>().Any(t => t.Name.Equals(namedMessage.Name)));

                    if (!methods.Any())
                    {
                        continue;
                    }

                    var handler = ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, handlerType);
                    foreach (var method in methods)
                    {
                        var parameters = GetMethodArguments(method, context, context, cancellationToken);
                        if (parameters == null)
                        {
                            _logger.LogWarning("Method '{Name}' parameter number not matches", method.Name);
                        }
                        else
                        {
                            tasks.Add(Invoke(method, handler, parameters));
                        }
                    }
                }
            }
        }
        else
        {
            var handlers = GetOrCreateHandlers(message, scope.ServiceProvider);

            if (handlers == null || !handlers.Any())
            {
                return;
            }

            var messageType = message.GetType();

            foreach (var handler in handlers)
            {
                if (!handler.CanHandle(messageType))
                {
                    continue;
                }

                _logger.LogInformation("Message {Id}({MessageType}) will be handled by {HandlerType}", message.Id, messageType.FullName, handler);

                tasks.Add(handler.HandleAsync(message, context, cancellationToken));
            }
        }

        if (tasks.Count == 0)
        {
            return;
        }

        await Task.WhenAll(tasks).ContinueWith(_ =>
        {
            _logger?.LogInformation("Message {Id} was completed handled", message.Id);
        }, cancellationToken);
    }

    #endregion

    #region Supports

    private IEnumerable<IMessageHandler> GetOrCreateHandlers(IMessage message, IServiceProvider provider)
    {
        var messageType = message.GetType();

        if (_handlerContainer.TryGetValue(messageType.FullName!, out var handlerTypes))
        {
            if (handlerTypes == null || handlerTypes.Count < 1)
            {
                yield break;
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var handlerType in handlerTypes)
            {
                var handler = (IMessageHandler)ActivatorUtilities.GetServiceOrCreateInstance(provider, handlerType);
                yield return handler;
            }
        }
        else
        {
            var required = message is Command;
            var interfaceType = message switch
            {
                Event _ => typeof(IEventHandler<>).MakeGenericType(messageType),
                Command _ => typeof(ICommandHandler<>).MakeGenericType(messageType),
                _ => throw new InvalidOperationException()
            };

            var handlers = provider.GetServices(interfaceType);
            if (required && (handlers == null || !handlers.Any()))
            {
                throw new InvalidOperationException($"No handler was found for {messageType.FullName}");
            }

            foreach (var handler in handlers)
            {
                yield return (IMessageHandler)handler;
            }
        }
    }

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

    private object[] GetMethodArguments(MethodInfo method, object message, MessageContext context, CancellationToken cancellationToken)
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
                    parameters[0] = _conversion(message, parameterType);
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

                parameters[0] ??= _conversion(message, parameterInfos[0].ParameterType);
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