using System.Reflection;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

public class MessageHandlerOptions
{
    private const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    /// <summary>
    /// The message handler types.
    /// </summary>
    public List<MessageSubscription> Subscription { get; } = new();

    /// <summary>
    /// Subscribes the specified message type.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="THandler"></typeparam>
    public void Subscribe<TMessage, THandler>()
        where TMessage : IMessage
        where THandler : IMessageHandler<TMessage>
    {
        Subscribe(typeof(TMessage), typeof(THandler));
    }

    /// <summary>
    /// Subscribes the specified message type.
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="handlerType"></param>
    public void Subscribe(Type messageType, Type handlerType)
    {
        if (!messageType.IsAssignableTo<IMessage>())
        {
            throw new InvalidOperationException($"The message must inherits type of {typeof(IEvent).FullName} or {typeof(ICommand).FullName}");
        }

        if (!handlerType.IsAssignableTo(typeof(IMessageHandler<>).MakeGenericType(messageType)))
        {
            throw new InvalidOperationException($"The message handler type must implements ICommandHandler<{messageType.Name}> or IEventHandler<{messageType.Name}>");
        }

        Subscription.Add(new MessageSubscription(messageType, handlerType));
    }

    /// <summary>
    /// Subscribes the specified message type.
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="handlerTypes"></param>
    public void Subscribe(Type messageType, IEnumerable<Type> handlerTypes)
    {
        foreach (var handlerType in handlerTypes)
        {
            Subscribe(messageType, handlerType);
        }
    }

    public void Subscribe(string messageName, Type handlerType)
    {
        Subscription.Add(new MessageSubscription(messageName, handlerType));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handlerType"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Subscribe(Type handlerType)
    {
        if (!handlerType.IsAssignableTo(typeof(IMessageHandler)))
        {
            throw new InvalidOperationException($"The message handler type must implements ICommandHandler<> or IEventHandler<>");
        }

        var messageTypes = from interfaceType in handlerType.GetInterfaces()
                           where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IMessageHandler<>)
                           select interfaceType.GetGenericArguments()[0];

        messageTypes = messageTypes.Distinct();

        foreach (var messageType in messageTypes)
        {
            if (!messageType.IsAssignableTo(typeof(IMessage)))
            {
                continue;
            }

            Subscribe(messageType, handlerType);
        }
    }

    /// <summary>
    /// Register all handlers in assembly.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    public void Subscribe(Assembly assembly)
    {
        var handlerTypes = from type in assembly.DefinedTypes
                           where type.IsClass && !type.IsAbstract // && type.IsAssignableTo(typeof(IMessageHandler))
                           select type;

        foreach (var handlerType in handlerTypes)
        {
            if (handlerType.IsAssignableTo(typeof(IMessageHandler)))
            {
                var messageTypes = from interfaceType in handlerType.ImplementedInterfaces
                                   where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IMessageHandler<>)
                                   select interfaceType.GetGenericArguments()[0];

                messageTypes = messageTypes.Distinct();

                foreach (var messageType in messageTypes)
                {
                    Subscribe(messageType, handlerType);
                }
            }
            else
            {
                var methods = handlerType.GetMethods(BINDING_FLAGS).Where(method => method.GetCustomAttributes<EventSubscribeAttribute>().Any());

                var attributes = methods.SelectMany(t => t.GetCustomAttributes<EventSubscribeAttribute>());

                foreach (var attribute in attributes)
                {
                    Subscribe(attribute.Name, handlerType);
                }
            }
        }
    }
}