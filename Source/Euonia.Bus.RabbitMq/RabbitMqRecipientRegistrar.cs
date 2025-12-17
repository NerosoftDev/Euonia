using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// The RabbitMQ recipient registrar.
/// </summary>
public sealed class RabbitMqRecipientRegistrar : IRecipientRegistrar
{
	private readonly IMessageConvention _convention;
	private readonly IServiceProvider _provider;
	private readonly ITransportStrategy _strategy;
	private readonly RabbitMqBusOptions _options;

	/// <summary>
	/// Initialize a new instance of <see cref="RabbitMqRecipientRegistrar"/>.
	/// </summary>
	/// <param name="convention"></param>
	/// <param name="provider"></param>
	/// <param name="options"></param>
	public RabbitMqRecipientRegistrar(IMessageConvention convention, IServiceProvider provider, IOptions<RabbitMqBusOptions> options)
	{
		_convention = convention;
		_provider = provider;
		_options = options.Value;
		_strategy = provider.GetKeyedService<ITransportStrategy>(_options.Name);
	}

	/// <inheritdoc/>
	public async Task RegisterAsync(IEnumerable<MessageRegistration> registrations, string defaultTransport, CancellationToken cancellationToken = default)
	{
		foreach (var registration in registrations)
		{
			if (!string.Equals(defaultTransport, _options.Name, StringComparison.CurrentCultureIgnoreCase))
			{
				// Check if the strategy allows incoming handling for the message type
				if (_strategy != null && !_strategy.Incoming(registration.MessageType))
				{
					return;
				}
			}

			RabbitMqQueueRecipient recipient;
			if (_convention.IsUnicastType(registration.MessageType))
			{
				recipient = ActivatorUtilities.GetServiceOrCreateInstance<RabbitMqQueueConsumer>(_provider);
			}
			else if (_convention.IsMulticastType(registration.MessageType))
			{
				recipient = ActivatorUtilities.GetServiceOrCreateInstance<RabbitMqTopicSubscriber>(_provider);
			}
			else if (_convention.IsRequestType(registration.MessageType))
			{
				recipient = ActivatorUtilities.GetServiceOrCreateInstance<RabbitMqQueueConsumer>(_provider);
			}
			else
			{
				throw new MessageTypeException($"The message type {registration.MessageType.AssemblyQualifiedName} is not a queue/topic/request type.");
			}

			await recipient.StartAsync(registration.Channel);
		}

		await Task.CompletedTask;
	}
}