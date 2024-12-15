using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// The RabbitMQ recipient registrar.
/// </summary>
public sealed class RabbitMqRecipientRegistrar : IRecipientRegistrar
{
	private readonly IMessageConvention _convention;
	private readonly IServiceProvider _provider;

	/// <summary>
	/// Initialize a new instance of <see cref="RabbitMqRecipientRegistrar"/>.
	/// </summary>
	/// <param name="convention"></param>
	/// <param name="provider"></param>
	public RabbitMqRecipientRegistrar(IMessageConvention convention, IServiceProvider provider)
	{
		_convention = convention;
		_provider = provider;
	}

	/// <inheritdoc/>
	public async Task RegisterAsync(IEnumerable<MessageRegistration> registrations, CancellationToken cancellationToken = default)
	{
		await Parallel.ForEachAsync(registrations, cancellationToken, async (registration, token) =>
		{
			RabbitMqQueueRecipient recipient;
			if (_convention.IsQueueType(registration.MessageType))
			{
				recipient = ActivatorUtilities.GetServiceOrCreateInstance<RabbitMqQueueConsumer>(_provider);
			}
			else if (_convention.IsTopicType(registration.MessageType))
			{
				recipient = ActivatorUtilities.GetServiceOrCreateInstance<RabbitMqTopicSubscriber>(_provider);
			}
			else
			{
				throw new InvalidOperationException("The message type is neither a queue nor a topic.");
			}

			await recipient.StartAsync(registration.Channel, token);
		});
	}
}