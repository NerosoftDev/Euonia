using System.Reflection;
using System.Runtime.CompilerServices;
// ReSharper disable UnusedType.Local
// ReSharper disable UnusedMember.Local

namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// Extensions for the <see cref="IMessenger"/> type.
/// </summary>
public static partial class MessengerExtensions
{
	/// <summary>
	/// A class that acts as a container to load the <see cref="MethodInfo"/> instance linked to
	/// the <see cref="Register{TMessage,TToken}(IMessenger,IRecipient{TMessage},TToken)"/> method.
	/// This class is needed to avoid forcing the initialization code in the static constructor to run as soon as
	/// the <see cref="MessengerExtensions"/> type is referenced, even if that is done just to use methods
	/// that do not actually require this <see cref="MethodInfo"/> instance to be available.
	/// We're effectively using this type to leverage the lazy loading of static constructors done by the runtime.
	/// </summary>
	private static class MethodInfos
	{
		/// <summary>
		/// The <see cref="MethodInfo"/> instance associated with <see cref="Register{TMessage,TToken}(IMessenger,IRecipient{TMessage},TToken)"/>.
		/// </summary>
		public static readonly MethodInfo RegisterIRecipient = new Action<IMessenger, IRecipient<object>, Unit>(Register).Method.GetGenericMethodDefinition();
	}

	/// <summary>
	/// A non-generic version of <see cref="DiscoveredRecipients{TToken}"/>.
	/// </summary>
	private static class DiscoveredRecipients
	{
		/// <summary>
		/// The <see cref="ConditionalWeakTable{TKey,TValue}"/> instance used to track the preloaded registration action for each recipient.
		/// </summary>
		public static readonly ConditionalWeakTable<Type, Action<IMessenger, object>> RegistrationMethods = new();
	}

	/// <summary>
	/// A class that acts as a static container to associate a <see cref="ConditionalWeakTable{TKey,TValue}"/> instance to each
	/// <typeparamref name="TToken"/> type in use. This is done because we can only use a single type as key, but we need to track
	/// associations of each recipient type also across different communication channels, each identified by a token.
	/// Since the token is actually a compile-time parameter, we can use a wrapping class to let the runtime handle a different
	/// instance for each generic type instantiation. This lets us only worry about the recipient type being inspected.
	/// </summary>
	/// <typeparam name="TToken">The token indicating what channel to use.</typeparam>
	private static class DiscoveredRecipients<TToken>
		where TToken : IEquatable<TToken>
	{
		/// <summary>
		/// The <see cref="ConditionalWeakTable{TKey,TValue}"/> instance used to track the preloaded registration action for each recipient.
		/// </summary>
		public static readonly ConditionalWeakTable<Type, Action<IMessenger, object, TToken>> RegistrationMethods = new();
	}

	/// <summary>
	/// Checks whether or not a given recipient has already been registered for a message.
	/// </summary>
	/// <typeparam name="TMessage">The type of message to check for the given recipient.</typeparam>
	/// <param name="messenger">The <see cref="IMessenger"/> instance to use to check the registration.</param>
	/// <param name="recipient">The target recipient to check the registration for.</param>
	/// <returns>Whether or not <paramref name="recipient"/> has already been registered for the specified message.</returns>
	/// <remarks>This method will use the default channel to check for the requested registration.</remarks>
	/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/> or <paramref name="recipient"/> are <see langword="null"/>.</exception>
	public static bool IsRegistered<TMessage>(this IMessenger messenger, object recipient)
		where TMessage : class
	{
		ArgumentAssert.ThrowIfNull(messenger);
		ArgumentAssert.ThrowIfNull(recipient);

		return messenger.IsRegistered<TMessage, Unit>(recipient, default);
	}

	/// <summary>
	/// Registers a recipient for a given type of message.
	/// </summary>
	/// <typeparam name="TMessage">The type of message to receive.</typeparam>
	/// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
	/// <param name="recipient">The recipient that will receive the messages.</param>
	/// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
	/// <remarks>This method will use the default channel to perform the requested registration.</remarks>
	/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/> or <paramref name="recipient"/> are <see langword="null"/>.</exception>
	public static void Register<TMessage>(this IMessenger messenger, IRecipient<TMessage> recipient)
		where TMessage : class
	{
		ArgumentAssert.ThrowIfNull(messenger);
		ArgumentAssert.ThrowIfNull(recipient);

		if (messenger is WeakReferenceMessenger weakReferenceMessenger)
		{
			weakReferenceMessenger.Register<TMessage, Unit>(recipient, default);
		}
		else if (messenger is StrongReferenceMessenger strongReferenceMessenger)
		{
			strongReferenceMessenger.Register<TMessage, Unit>(recipient, default);
		}
		else
		{
			messenger.Register<IRecipient<TMessage>, TMessage, Unit>(recipient, default, static (r, m) => r.Receive(m));
		}
	}

	/// <summary>
	/// Registers a recipient for a given type of message.
	/// </summary>
	/// <typeparam name="TMessage">The type of message to receive.</typeparam>
	/// <typeparam name="TToken">The type of token to identify what channel to use to receive messages.</typeparam>
	/// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
	/// <param name="recipient">The recipient that will receive the messages.</param>
	/// <param name="token">The token indicating what channel to use.</param>
	/// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
	/// <remarks>This method will use the default channel to perform the requested registration.</remarks>
	/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/>, <paramref name="recipient"/> or <paramref name="token"/> are <see langword="null"/>.</exception>
	public static void Register<TMessage, TToken>(this IMessenger messenger, IRecipient<TMessage> recipient, TToken token)
		where TMessage : class
		where TToken : IEquatable<TToken>
	{
		ArgumentAssert.ThrowIfNull(messenger);
		ArgumentAssert.ThrowIfNull(recipient);
		ArgumentAssert.For<TToken>.ThrowIfNull(token);

		if (messenger is WeakReferenceMessenger weakReferenceMessenger)
		{
			weakReferenceMessenger.Register(recipient, token);
		}
		else if (messenger is StrongReferenceMessenger strongReferenceMessenger)
		{
			strongReferenceMessenger.Register(recipient, token);
		}
		else
		{
			messenger.Register<IRecipient<TMessage>, TMessage, TToken>(recipient, token, static (r, m) => r.Receive(m));
		}
	}

	/// <summary>
	/// Registers a recipient for a given type of message.
	/// </summary>
	/// <typeparam name="TMessage">The type of message to receive.</typeparam>
	/// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
	/// <param name="recipient">The recipient that will receive the messages.</param>
	/// <param name="handler">The <see cref="MessageHandler{TRecipient,TMessage}"/> to invoke when a message is received.</param>
	/// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
	/// <remarks>This method will use the default channel to perform the requested registration.</remarks>
	/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/>, <paramref name="recipient"/> or <paramref name="handler"/> are <see langword="null"/>.</exception>
	public static void Register<TMessage>(this IMessenger messenger, object recipient, MessageHandler<object, TMessage> handler)
		where TMessage : class
	{
		ArgumentAssert.ThrowIfNull(messenger);
		ArgumentAssert.ThrowIfNull(recipient);
		ArgumentAssert.ThrowIfNull(handler);

		messenger.Register(recipient, default(Unit), handler);
	}

	/// <summary>
	/// Registers a recipient for a given type of message.
	/// </summary>
	/// <typeparam name="TRecipient">The type of recipient for the message.</typeparam>
	/// <typeparam name="TMessage">The type of message to receive.</typeparam>
	/// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
	/// <param name="recipient">The recipient that will receive the messages.</param>
	/// <param name="handler">The <see cref="MessageHandler{TRecipient,TMessage}"/> to invoke when a message is received.</param>
	/// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
	/// <remarks>This method will use the default channel to perform the requested registration.</remarks>
	/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/>, <paramref name="recipient"/> or <paramref name="handler"/> are <see langword="null"/>.</exception>
	public static void Register<TRecipient, TMessage>(this IMessenger messenger, TRecipient recipient, MessageHandler<TRecipient, TMessage> handler)
		where TRecipient : class
		where TMessage : class
	{
		ArgumentAssert.ThrowIfNull(messenger);
		ArgumentAssert.ThrowIfNull(recipient);
		ArgumentAssert.ThrowIfNull(handler);

		messenger.Register(recipient, default(Unit), handler);
	}

	/// <summary>
	/// Registers a recipient for a given type of message.
	/// </summary>
	/// <typeparam name="TMessage">The type of message to receive.</typeparam>
	/// <typeparam name="TToken">The type of token to use to pick the messages to receive.</typeparam>
	/// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
	/// <param name="recipient">The recipient that will receive the messages.</param>
	/// <param name="token">A token used to determine the receiving channel to use.</param>
	/// <param name="handler">The <see cref="MessageHandler{TRecipient,TMessage}"/> to invoke when a message is received.</param>
	/// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
	/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/>, <paramref name="recipient"/> or <paramref name="handler"/> are <see langword="null"/>.</exception>
	public static void Register<TMessage, TToken>(this IMessenger messenger, object recipient, TToken token, MessageHandler<object, TMessage> handler)
		where TMessage : class
		where TToken : IEquatable<TToken>
	{
		ArgumentAssert.ThrowIfNull(messenger);
		ArgumentAssert.ThrowIfNull(recipient);
		ArgumentAssert.For<TToken>.ThrowIfNull(token);
		ArgumentAssert.ThrowIfNull(handler);

		messenger.Register(recipient, token, handler);
	}

	/// <summary>
	/// Unregisters a recipient from messages of a given type.
	/// </summary>
	/// <typeparam name="TMessage">The type of message to stop receiving.</typeparam>
	/// <param name="messenger">The <see cref="IMessenger"/> instance to use to unregister the recipient.</param>
	/// <param name="recipient">The recipient to unregister.</param>
	/// <remarks>
	/// This method will unregister the target recipient only from the default channel.
	/// If the recipient has no registered handler, this method does nothing.
	/// </remarks>
	/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/> or <paramref name="recipient"/> are <see langword="null"/>.</exception>
	public static void Unregister<TMessage>(this IMessenger messenger, object recipient)
		where TMessage : class
	{
		ArgumentAssert.ThrowIfNull(messenger);
		ArgumentAssert.ThrowIfNull(recipient);

		messenger.Unregister<TMessage, Unit>(recipient, default);
	}

	/// <summary>
	/// Sends a message of the specified type to all registered recipients.
	/// </summary>
	/// <typeparam name="TMessage">The type of message to send.</typeparam>
	/// <param name="messenger">The <see cref="IMessenger"/> instance to use to send the message.</param>
	/// <returns>The message that has been sent.</returns>
	/// <remarks>
	/// This method is a shorthand for <see cref="Send{TMessage}(IMessenger,TMessage)"/> when the
	/// message type exposes a parameterless constructor: it will automatically create
	/// a new <typeparamref name="TMessage"/> instance and send that to its recipients.
	/// </remarks>
	/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/> is <see langword="null"/>.</exception>
	public static TMessage Send<TMessage>(this IMessenger messenger)
		where TMessage : class, new()
	{
		ArgumentAssert.ThrowIfNull(messenger);

		return messenger.Send(new TMessage(), default(Unit));
	}

	/// <summary>
	/// Sends a message of the specified type to all registered recipients.
	/// </summary>
	/// <typeparam name="TMessage">The type of message to send.</typeparam>
	/// <param name="messenger">The <see cref="IMessenger"/> instance to use to send the message.</param>
	/// <param name="message">The message to send.</param>
	/// <returns>The message that was sent (ie. <paramref name="message"/>).</returns>
	/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/> or <paramref name="message"/> are <see langword="null"/>.</exception>
	public static TMessage Send<TMessage>(this IMessenger messenger, TMessage message)
		where TMessage : class
	{
		ArgumentAssert.ThrowIfNull(messenger);
		ArgumentAssert.ThrowIfNull(message);

		return messenger.Send(message, default(Unit));
	}

	/// <summary>
	/// Sends a message of the specified type to all registered recipients.
	/// </summary>
	/// <typeparam name="TMessage">The type of message to send.</typeparam>
	/// <typeparam name="TToken">The type of token to identify what channel to use to send the message.</typeparam>
	/// <param name="messenger">The <see cref="IMessenger"/> instance to use to send the message.</param>
	/// <param name="token">The token indicating what channel to use.</param>
	/// <returns>The message that has been sen.</returns>
	/// <remarks>
	/// This method will automatically create a new <typeparamref name="TMessage"/> instance
	/// just like <see cref="Send{TMessage}(IMessenger)"/>, and then send it to the right recipients.
	/// </remarks>
	/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/> or <paramref name="token"/> are <see langword="null"/>.</exception>
	public static TMessage Send<TMessage, TToken>(this IMessenger messenger, TToken token)
		where TMessage : class, new()
		where TToken : IEquatable<TToken>
	{
		ArgumentAssert.ThrowIfNull(messenger);
		ArgumentAssert.For<TToken>.ThrowIfNull(token);

		return messenger.Send(new TMessage(), token);
	}
}