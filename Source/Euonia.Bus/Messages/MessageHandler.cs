namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public delegate Task MessageHandler<in TMessage>(TMessage message, MessageContext context, CancellationToken cancellationToken = default)
	where TMessage : class;

/// <summary>
/// 
/// </summary>
public delegate Task MessageHandler(object message, MessageContext context, CancellationToken cancellationToken = default);