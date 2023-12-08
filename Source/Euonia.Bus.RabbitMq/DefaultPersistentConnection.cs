using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// The default implementation of <see cref="IPersistentConnection"/>.
/// </summary>
public class DefaultPersistentConnection : DisposableObject, IPersistentConnection
{
	private readonly IConnectionFactory _connectionFactory;
	private readonly ILogger<DefaultPersistentConnection> _logger;
	private readonly int _retryCount;
	private IConnection _connection;
	private readonly object _lockObject = new();

	private bool IsDisposed { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="DefaultPersistentConnection"/> class.
	/// </summary>
	/// <param name="connectionFactory"></param>
	/// <param name="logger"></param>
	/// <param name="options"></param>
	public DefaultPersistentConnection(IConnectionFactory connectionFactory, ILoggerFactory logger, IOptions<RabbitMqMessageBusOptions> options)
	{
		_connectionFactory = connectionFactory;
		_logger = logger.CreateLogger<DefaultPersistentConnection>();
		_retryCount = options.Value.MaxFailureRetries;
	}

	/// <inheritdoc/>
	public bool IsConnected => _connection is { IsOpen: true } && !IsDisposed;

	/// <inheritdoc/>
	public bool TryConnect()
	{
		_logger.LogInformation("RabbitMQ Client is trying to connect");
		lock (_lockObject)
		{
			Policy.Handle<SocketException>()
			      .Or<BrokerUnreachableException>()
			      .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
			      {
				      _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
			      })
			      .Execute(() =>
			      {
				      _connection = _connectionFactory.CreateConnection();
			      });

			if (IsConnected)
			{
				_connection.ConnectionShutdown += OnConnectionShutdown;
				_connection.CallbackException += OnCallbackException;
				_connection.ConnectionBlocked += OnConnectionBlocked;

				_logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);

				return true;
			}
			else
			{
				_logger.LogCritical("Fatal error: RabbitMQ connections could not be created and opened");

				return false;
			}
		}
	}

	/// <inheritdoc/>
	public IModel CreateChannel()
	{
		if (!IsConnected)
		{
			throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
		}

		return _connection.CreateModel();
	}

	private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
	{
		if (IsDisposed)
		{
			return;
		}

		_logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

		TryConnect();
	}

	private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
	{
		if (IsDisposed)
		{
			return;
		}

		_logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

		TryConnect();
	}

	private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
	{
		if (IsDisposed)
		{
			return;
		}

		_logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

		TryConnect();
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if (IsDisposed)
		{
			return;
		}

		IsDisposed = true;

		try
		{
			_connection.ConnectionShutdown -= OnConnectionShutdown;
			_connection.CallbackException -= OnCallbackException;
			_connection.ConnectionBlocked -= OnConnectionBlocked;
			_connection.Dispose();
		}
		catch (IOException exception)
		{
			_logger.LogCritical(exception, "{Message}", exception.Message);
		}
	}
}