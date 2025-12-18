using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Modularity;
using Nerosoft.Euonia.Pipeline;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The implementation of <see cref="IBus"/> interface.
/// </summary>
public sealed class MessageBus : IBus
{
	private readonly ILogger<MessageBus> _logger;

	private readonly IDispatcher _dispatcher;
	private readonly IMessageConvention _convention;
	private readonly IRequestContextAccessor _requestAccessor;
	private readonly IServiceProvider _provider;
	private readonly MessageBusOptions _options;

	public MessageBus(IDispatcher dispatcher, IServiceProvider provider, IMessageConvention convention, ILoggerFactory logger, IOptionsMonitor<MessageBusOptions> options)
	{
		_logger = logger.CreateLogger<MessageBus>();
		_dispatcher = dispatcher;
		_convention = convention;
		_provider = provider;
		_options = options.CurrentValue;
		options.OnChange(opt =>
		{
			_options.DefaultTransport = opt.DefaultTransport;
			_options.EnablePipelineBehaviors = opt.EnablePipelineBehaviors;
		});
	}

	public MessageBus(IDispatcher dispatcher, IServiceProvider provider, IMessageConvention convention, ILoggerFactory logger, IOptionsMonitor<MessageBusOptions> options, IRequestContextAccessor requestAccessor)
		: this(dispatcher, provider, convention, logger, options)
	{
		_requestAccessor = requestAccessor;
	}

	public async Task PublishAsync<TMessage>(TMessage message, Action<PipelineMessage<IRoutedMessage, Unit>> behavior, PublishOptions options, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		options ??= new PublishOptions();

		var messageType = message.GetType();

		if (!_convention.IsMulticastType(messageType))
		{
			throw new MessageTypeException("The message type is not an topic type.");
		}

		var context = _requestAccessor?.Context;

		var channelName = options.Channel ?? MessageCache.Default.GetOrAddChannel(messageType);
		var pack = new RoutedMessage<TMessage, Unit>(message, channelName)
		{
			MessageId = options.MessageId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString("N"),
			Authorization = context?.Authorization,
		};

		metadataSetter?.Invoke(pack.Metadata);

		if (CheckPipelineBehaviorEnabled(options))
		{
			var pipeline = _provider.GetRequiredService<IPipeline<IRoutedMessage, Unit>>();

			var pipelineMessage = new PipelineMessage<IRoutedMessage, Unit>(pack, pipeline);

			if (options.AttachDefaultPipelineBehaviors)
			{
				pipelineMessage.Pipeline.UseOf(messageType, true);
				var behaviorTypes = MessageBusPipelineBehaviorTypeCache.Instance.GetOrAdd<Unit>(_provider);

				foreach (var behaviorType in behaviorTypes)
				{
					pipelineMessage.Pipeline.Use(behaviorType);
				}
			}

			if (behavior != null)
			{
				behavior(pipelineMessage);
			}

			await pipelineMessage.ExecuteAsync();
			pack = (RoutedMessage<TMessage, Unit>)pipelineMessage.Message;
		}

		var transports = _dispatcher.Determine(messageType);

		var tasks = new List<Task>();

		foreach (var name in transports)
		{
			_logger.LogDebug("Publishing message of type {MessageType} to transport {TransportType} on channel {ChannelName} with MessageId {MessageId}.",
				messageType.FullName, name, channelName, pack.MessageId);
			var transport = _provider.GetKeyedService<ITransport>(name);
			if (transport == null)
			{
				throw new MessageTransportException($"The transport '{name}' is not registered.");
			}
			tasks.Add(transport.PublishAsync(pack, cancellationToken));
		}

		await Task.WhenAll(tasks);
	}

	public async Task SendAsync<TMessage, TResult>(TMessage message, Action<PipelineMessage<IRoutedMessage, TResult>> behavior, Action<TResult> callback = null, SendOptions options = null, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		options ??= new SendOptions();

		var messageType = message.GetType();

		if (!_convention.IsUnicastType(messageType) && !_convention.IsRequestType(messageType))
		{
			throw new MessageTypeException("The message type is not a command type or request type.");
		}

		var context = _requestAccessor?.Context;

		var channelName = options.Channel ?? MessageCache.Default.GetOrAddChannel(messageType);
		var pack = new RoutedMessage<TMessage, TResult>(message, channelName)
		{
			MessageId = options.MessageId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			CorrelationId = options.CorrelationId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString("N"),
			Authorization = context?.Authorization,
		};

		metadataSetter?.Invoke(pack.Metadata);

		if (CheckPipelineBehaviorEnabled(options))
		{
			var pipeline = _provider.GetRequiredService<IPipeline<IRoutedMessage, TResult>>();

			var pipelineMessage = new PipelineMessage<IRoutedMessage, TResult>(pack, pipeline);

			if (options.AttachDefaultPipelineBehaviors)
			{
				pipelineMessage.Pipeline.UseOf(messageType, true);

				var behaviorTypes = MessageBusPipelineBehaviorTypeCache.Instance.GetOrAdd<TResult>(_provider);

				foreach (var behaviorType in behaviorTypes)
				{
					pipelineMessage.Pipeline.Use(behaviorType);
				}
			}

			if (behavior != null)
			{
				behavior(pipelineMessage);
			}

			await pipelineMessage.ExecuteAsync();
			pack = (RoutedMessage<TMessage, TResult>)pipelineMessage.Message;
		}

		var transports = _dispatcher.Determine(messageType);

		var transportName = transports!.First();

		_logger.LogDebug("Publishing message of type {MessageType} to transport {TransportType} on channel {ChannelName} with MessageId {MessageId}.",
			messageType.FullName, transportName, channelName, pack.MessageId);

		var transport = _provider.GetKeyedService<ITransport>(transportName);

		if (transport == null)
		{
			throw new MessageTransportException($"The transport '{transportName}' is not registered.");
		}

		await transport.SendAsync(pack, cancellationToken)
					   .ContinueWith(task =>
					   {
						   task.WaitAndUnwrapException();
						   var result = task.Result;
						   callback?.Invoke(result);
					   }, cancellationToken);
	}

	public async Task<TResult> CallAsync<TResult>(IRequest<TResult> message, Action<PipelineMessage<IRoutedMessage, TResult>> behavior, CallOptions options, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default)
	{
		options ??= new CallOptions();

		var messageType = message.GetType();

		if (!_convention.IsRequestType(messageType))
		{
			throw new MessageTypeException("The message type is not a queue type.");
		}

		var context = _requestAccessor?.Context;

		var channelName = options.Channel ?? MessageCache.Default.GetOrAddChannel(messageType);
		var pack = new RoutedMessage<IRequest<TResult>, TResult>(message, channelName)
		{
			MessageId = options.MessageId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			CorrelationId = options.CorrelationId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString("N"),
			Authorization = context?.Authorization,
		};

		metadataSetter?.Invoke(pack.Metadata);

		if (CheckPipelineBehaviorEnabled(options))
		{
			var pipeline = _provider.GetRequiredService<IPipeline<IRoutedMessage, TResult>>();

			var pipelineMessage = new PipelineMessage<IRoutedMessage, TResult>(pack, pipeline);

			if (options.AttachDefaultPipelineBehaviors)
			{
				pipelineMessage.Pipeline.UseOf(messageType, true);
				var behaviorTypes = MessageBusPipelineBehaviorTypeCache.Instance.GetOrAdd<TResult>(_provider);

				foreach (var behaviorType in behaviorTypes)
				{
					pipelineMessage.Pipeline.Use(behaviorType);
				}
			}

			if (behavior != null)
			{
				behavior(pipelineMessage);
			}

			await pipelineMessage.ExecuteAsync();
			pack = (RoutedMessage<IRequest<TResult>, TResult>)pipelineMessage.Message;
		}

		var transports = _dispatcher.Determine(messageType);

		var transportName = transports!.First();

		_logger.LogDebug("Publishing message of type {MessageType} to transport {TransportType} on channel {ChannelName} with MessageId {MessageId}.",
			messageType.FullName, transportName, channelName, pack.MessageId);

		var transport = _provider.GetKeyedService<ITransport>(transportName);

		if (transport == null)
		{
			throw new MessageTransportException($"The transport '{transportName}' is not registered.");
		}

		var result = await transport.SendAsync(pack, cancellationToken)
									.ContinueWith(task =>
									{
										task.WaitAndUnwrapException();
										return task.Result;
									}, cancellationToken);
		return result;
	}

	private bool CheckPipelineBehaviorEnabled(ExtendableOptions options)
	{
		if (options.EnablePipelineBehaviors.HasValue)
		{
			return options.EnablePipelineBehaviors.Value;
		}
		return _options.EnablePipelineBehaviors;
	}
}

public class MessageBusPipelineBehaviorTypeCache
{
	private readonly ConcurrentDictionary<Type, List<Type>> _cache = new();

	public static MessageBusPipelineBehaviorTypeCache Instance => Singleton<MessageBusPipelineBehaviorTypeCache>.Get(() => new());

	public List<Type> GetOrAdd<TResponse>(IServiceProvider provider)
	{
		return _cache.GetOrAdd(typeof(TResponse), _ =>
		{
			return provider.GetServices<IPipelineBehavior<IRoutedMessage, TResponse>>()
				  .Select(b => b.GetType())
				  .Distinct()
				  .ToList();
		});
	}
}