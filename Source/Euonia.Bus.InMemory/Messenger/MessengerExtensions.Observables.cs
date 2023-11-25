namespace Nerosoft.Euonia.Bus.InMemory;

partial class MessengerExtensions
{
	/// <summary>
	/// Creates an <see cref="IObservable{T}"/> instance that can be used to be notified whenever a message of a given type is broadcast by a messenger.
	/// </summary>
	/// <typeparam name="TMessage">The type of message to use to receive notification for through the resulting <see cref="IObservable{T}"/> instance.</typeparam>
	/// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
	/// <returns>An <see cref="IObservable{T}"/> instance to receive notifications for <typeparamref name="TMessage"/> messages being broadcast.</returns>
	/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/> is <see langword="null"/>.</exception>
	public static IObservable<TMessage> CreateObservable<TMessage>(this IMessenger messenger)
		where TMessage : class
	{
		ArgumentAssert.ThrowIfNull(messenger);

		return new Observable<TMessage>(messenger);
	}

	/// <summary>
	/// Creates an <see cref="IObservable{T}"/> instance that can be used to be notified whenever a message of a given type is broadcast by a messenger.
	/// </summary>
	/// <typeparam name="TMessage">The type of message to use to receive notification for through the resulting <see cref="IObservable{T}"/> instance.</typeparam>
	/// <typeparam name="TToken">The type of token to identify what channel to use to receive messages.</typeparam>
	/// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
	/// <param name="token">A token used to determine the receiving channel to use.</param>
	/// <returns>An <see cref="IObservable{T}"/> instance to receive notifications for <typeparamref name="TMessage"/> messages being broadcast.</returns>
	/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/> or <paramref name="token"/> are <see langword="null"/>.</exception>
	public static IObservable<TMessage> CreateObservable<TMessage, TToken>(this IMessenger messenger, TToken token)
		where TMessage : class
		where TToken : IEquatable<TToken>
	{
		ArgumentAssert.ThrowIfNull(messenger);
		ArgumentAssert.For<TToken>.ThrowIfNull(token);

		return new Observable<TMessage, TToken>(messenger, token);
	}

	/// <summary>
	/// An <see cref="IObservable{T}"/> implementations for a given message type.
	/// </summary>
	/// <typeparam name="TMessage">The type of messages to listen to.</typeparam>
	private sealed class Observable<TMessage> : IObservable<TMessage>
		where TMessage : class
	{
		/// <summary>
		/// The <see cref="IMessenger"/> instance to use to register the recipient.
		/// </summary>
		private readonly IMessenger _messenger;

		/// <summary>
		/// Creates a new <see cref="Observable{TMessage}"/> instance with the given parameters.
		/// </summary>
		/// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
		public Observable(IMessenger messenger)
		{
			_messenger = messenger;
		}

		/// <inheritdoc/>
		public IDisposable Subscribe(IObserver<TMessage> observer)
		{
			return new Recipient(_messenger, observer);
		}

		/// <summary>
		/// An <see cref="IRecipient{TMessage}"/> implementation for <see cref="Observable{TMessage}"/>.
		/// </summary>
		private sealed class Recipient : IRecipient<TMessage>, IDisposable
		{
			/// <summary>
			/// The <see cref="IMessenger"/> instance to use to register the recipient.
			/// </summary>
			private readonly IMessenger _messenger;

			/// <summary>
			/// The target <see cref="IObserver{T}"/> instance currently in use.
			/// </summary>
			private readonly IObserver<TMessage> _observer;

			/// <summary>
			/// Creates a new <see cref="Recipient"/> instance with the specified parameters.
			/// </summary>
			/// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
			/// <param name="observer">The <see cref="IObserver{T}"/> instance to use to create the recipient for.</param>
			public Recipient(IMessenger messenger, IObserver<TMessage> observer)
			{
				_messenger = messenger;
				_observer = observer;

				messenger.Register(this);
			}

			/// <inheritdoc/>
			public void Receive(TMessage pack)
			{
				_observer.OnNext(pack);
			}

			/// <inheritdoc/>
			public void Dispose()
			{
				_messenger.Unregister<TMessage>(this);
			}
		}
	}

	/// <summary>
	/// An <see cref="IObservable{T}"/> implementations for a given pair of message and token types.
	/// </summary>
	/// <typeparam name="TMessage">The type of messages to listen to.</typeparam>
	/// <typeparam name="TToken">The type of token to identify what channel to use to receive messages.</typeparam>
	private sealed class Observable<TMessage, TToken> : IObservable<TMessage>
		where TMessage : class
		where TToken : IEquatable<TToken>
	{
		/// <summary>
		/// The <see cref="IMessenger"/> instance to use to register the recipient.
		/// </summary>
		private readonly IMessenger _messenger;

		/// <summary>
		/// The token used to determine the receiving channel to use.
		/// </summary>
		private readonly TToken _token;

		/// <summary>
		/// Creates a new <see cref="Observable{TMessage, TToken}"/> instance with the given parameters.
		/// </summary>
		/// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
		/// <param name="token">A token used to determine the receiving channel to use.</param>
		public Observable(IMessenger messenger, TToken token)
		{
			_messenger = messenger;
			_token = token;
		}

		/// <inheritdoc/>
		public IDisposable Subscribe(IObserver<TMessage> observer)
		{
			return new Recipient(_messenger, observer, _token);
		}

		/// <summary>
		/// An <see cref="IRecipient{TMessage}"/> implementation for <see cref="Observable{TMessage, TToken}"/>.
		/// </summary>
		private sealed class Recipient : IRecipient<TMessage>, IDisposable
		{
			/// <summary>
			/// The <see cref="IMessenger"/> instance to use to register the recipient.
			/// </summary>
			private readonly IMessenger _messenger;

			/// <summary>
			/// The target <see cref="IObserver{T}"/> instance currently in use.
			/// </summary>
			private readonly IObserver<TMessage> _observer;

			/// <summary>
			/// The token used to determine the receiving channel to use.
			/// </summary>
			private readonly TToken _token;

			/// <summary>
			/// Creates a new <see cref="Recipient"/> instance with the specified parameters.
			/// </summary>
			/// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
			/// <param name="observer">The <see cref="IObserver{T}"/> instance to use to create the recipient for.</param>
			/// <param name="token">A token used to determine the receiving channel to use.</param>
			public Recipient(IMessenger messenger, IObserver<TMessage> observer, TToken token)
			{
				_messenger = messenger;
				_observer = observer;
				_token = token;

				messenger.Register(this, token);
			}

			/// <inheritdoc/>
			public void Receive(TMessage pack)
			{
				_observer.OnNext(pack);
			}

			/// <inheritdoc/>
			public void Dispose()
			{
				_messenger.Unregister<TMessage, TToken>(this, _token);
			}
		}
	}
}