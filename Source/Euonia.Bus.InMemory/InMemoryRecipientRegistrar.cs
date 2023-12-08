using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// The in-memory message recipient registrar.
/// </summary>
public sealed class InMemoryRecipientRegistrar : IRecipientRegistrar
{
	private readonly InMemoryBusOptions _options;
	private readonly IMessageConvention _convention;
	private readonly IServiceProvider _provider;

	/// <summary>
	/// Initializes a new instance of the <see cref="InMemoryRecipientRegistrar"/>.
	/// </summary>
	/// <param name="convention"></param>
	/// <param name="provider"></param>
	/// <param name="options"></param>
	/// 
	public InMemoryRecipientRegistrar(IMessageConvention convention, IServiceProvider provider, IOptions<InMemoryBusOptions> options)
	{
		_options = options.Value;
		_convention = convention;
		_provider = provider;
	}

	/// <inheritdoc/>
	public async Task RegisterAsync(IEnumerable<MessageRegistration> registrations, CancellationToken cancellationToken = default)
	{
		var recipients = new ConcurrentDictionary<Type, InMemoryRecipient>();

		foreach (var registration in registrations)
		{
			if (_convention.IsQueueType(registration.MessageType))
			{
				var recipient = GetRecipient<InMemoryQueueConsumer>();
				StrongReferenceMessenger.Default.Register(recipient, registration.Channel);
			}
			else if (_convention.IsTopicType(registration.MessageType))
			{
				var recipient = GetRecipient<InMemoryTopicSubscriber>();
				WeakReferenceMessenger.Default.Register(recipient, registration.Channel);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		TRecipient GetRecipient<TRecipient>()
			where TRecipient : InMemoryRecipient, IRecipient
		{
			if (_options.MultipleSubscriberInstance)
			{
				return _provider.GetService<TRecipient>();
			}
			else
			{
				return (TRecipient)recipients.GetOrAdd(typeof(TRecipient), _ => _provider.GetService<TRecipient>());
			}
		}

		await Task.CompletedTask;
	}
}
