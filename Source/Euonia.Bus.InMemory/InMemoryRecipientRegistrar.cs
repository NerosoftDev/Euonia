using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Nerosoft.Euonia.Bus.InMemory;

public sealed class InMemoryRecipientRegistrar : IRecipientRegistrar
{
	private readonly InMemoryBusOptions _options;
	private readonly IMessageConvention _convention;
	private readonly IServiceProvider _provider;
	private readonly IMessenger _messenger;

	public InMemoryRecipientRegistrar(IMessenger messenger, IMessageConvention convention, IServiceProvider provider, IOptions<InMemoryBusOptions> options)
	{
		_options = options.Value;
		_convention = convention;
		_provider = provider;
		_messenger = messenger;
	}

	public async Task RegisterAsync(IReadOnlyList<MessageRegistration> registrations, CancellationToken cancellationToken = default)
	{
		if (_options.MultipleSubscriberInstance)
		{
			foreach (var registration in registrations)
			{
				InMemoryRecipient recipient;
				if (_convention.IsQueueType(registration.MessageType))
				{
					recipient = ActivatorUtilities.GetServiceOrCreateInstance<InMemoryQueueConsumer>(_provider);
				}
				else if (_convention.IsTopicType(registration.MessageType))
				{
					recipient = ActivatorUtilities.GetServiceOrCreateInstance<InMemoryTopicSubscriber>(_provider);
				}
				else
				{
					throw new InvalidOperationException();
				}
				_messenger.Register(recipient, registration.Channel);
			}
		}
		else
		{
			foreach (var registration in registrations)
			{
				InMemoryRecipient recipient;
				if (_convention.IsQueueType(registration.MessageType))
				{
					recipient = Singleton<InMemoryQueueConsumer>.Get(() => ActivatorUtilities.GetServiceOrCreateInstance<InMemoryQueueConsumer>(_provider));
				}
				else if (_convention.IsTopicType(registration.MessageType))
				{
					recipient = Singleton<InMemoryTopicSubscriber>.Get(() => ActivatorUtilities.GetServiceOrCreateInstance<InMemoryTopicSubscriber>(_provider));
				}
				else
				{
					throw new InvalidOperationException();
				}
				_messenger.Register(recipient, registration.Channel);
			}
		}
	}
}
