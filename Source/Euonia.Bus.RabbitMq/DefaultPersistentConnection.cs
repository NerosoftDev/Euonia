using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Threading;
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
	private readonly AsyncLock _mutex = new();

	private readonly IConnectionFactory _connectionFactory;
	private readonly ILogger<DefaultPersistentConnection> _logger;
	private readonly int _retryCount;
	private IConnection _connection;

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
	public async Task<bool> TryConnectAsync()
	{
		using (await _mutex.LockAsync())
		{
			if (IsConnected)
			{
				return true;
			}

			_logger.LogInformation("RabbitMQ Client is trying to connect");
			_connection = await Policy.Handle<SocketException>()
			                          .Or<BrokerUnreachableException>()
			                          .WaitAndRetryAsync(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
			                          {
				                          _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
			                          })
			                          .ExecuteAsync(() => _connectionFactory.CreateConnectionAsync());

			if (IsConnected)
			{
				_connection.ConnectionShutdownAsync += OnConnectionShutdown;
				_connection.CallbackExceptionAsync += OnCallbackException;
				_connection.ConnectionBlockedAsync += OnConnectionBlocked;

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
	public async Task<IChannel> CreateChannelAsync()
	{
		if (!IsConnected)
		{
			await TryConnectAsync();
			//throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
		}

		return await _connection.CreateChannelAsync();
	}

	private async Task OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
	{
		if (IsDisposed)
		{
			return;
		}

		_logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

		await TryConnectAsync();
	}

	private async Task OnCallbackException(object sender, CallbackExceptionEventArgs e)
	{
		if (IsDisposed)
		{
			return;
		}

		_logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

		await TryConnectAsync();
	}

	private async Task OnConnectionShutdown(object sender, ShutdownEventArgs reason)
	{
		if (IsDisposed)
		{
			return;
		}

		_logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

		await TryConnectAsync();
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
			_connection.ConnectionShutdownAsync -= OnConnectionShutdown;
			_connection.CallbackExceptionAsync -= OnCallbackException;
			_connection.ConnectionBlockedAsync -= OnConnectionBlocked;
			_connection.Dispose();
		}
		catch (IOException exception)
		{
			_logger.LogCritical(exception, "{Message}", exception.Message);
		}
	}
}