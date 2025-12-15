using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// The RabbitMQ recipient registrar.
/// </summary>
public sealed class RabbitMqRecipientRegistrar : IRecipientRegistrar
{
	private readonly IMessageConvention _convention;
	private readonly IServiceProvider _provider;
	private readonly ITransportStrategy _strategy;

	/// <summary>
	/// Initialize a new instance of <see cref="RabbitMqRecipientRegistrar"/>.
	/// </summary>
	/// <param name="convention"></param>
	/// <param name="provider"></param>
	public RabbitMqRecipientRegistrar(IMessageConvention convention, IServiceProvider provider)
	{
		_convention = convention;
		_provider = provider;
		_strategy = provider.GetKeyedService<ITransportStrategy>(typeof(RabbitMqTransport));
	}

	/// <inheritdoc/>
	public async Task RegisterAsync(IEnumerable<MessageRegistration> registrations, CancellationToken cancellationToken = default)
	{
		foreach (var registration in registrations)
		{
			// Check if the strategy allows inbound handling for the message type
			if (_strategy != null && !_strategy.Inbound(registration.MessageType))
			{
				return;
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