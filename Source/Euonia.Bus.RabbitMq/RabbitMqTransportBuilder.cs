using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// 
/// </summary>
public class RabbitMqTransportBuilder : ITransportBuilder
{
	private readonly IServiceCollection _services;

	internal RabbitMqTransportBuilder(IServiceCollection services)
	{
		_services = services;
	}

	/// <summary>
	/// Adds the RabbitMQ message transporter.
	/// </summary>
	/// <param name="identifier"></param>
	/// <param name="configureConnection"></param>
	/// <param name="configureOptions"></param>
	/// <returns></returns>
	public RabbitMqTransportBuilder AddTransport(string identifier, Action<IConnectionFactory> configureConnection, Action<RabbitMqMessageBusOptions> configureOptions)
	{
		var options = new RabbitMqMessageBusOptions();
		configureOptions(options);

		_services.AddKeyedSingleton<IPersistentConnection>(identifier, (provider, _) =>
		{
			var connection = new ConnectionFactory();
			configureConnection(connection);
			return ActivatorUtilities.CreateInstance<DefaultPersistentConnection>(provider, connection, Options.Create(options));
		});

		_services.AddKeyedSingleton<ITransport, RabbitMqTransport>(identifier, (provider, _) =>
		{
			var connection = provider.GetKeyedService<IPersistentConnection>(identifier);
			return ActivatorUtilities.CreateInstance<RabbitMqTransport>(provider, connection, options);
		});
		return this;
	}

	public RabbitMqTransportBuilder When(Func<Type, bool> strategy)
	{
		return this;
	}

	public RabbitMqTransportBuilder When(Func<string, bool> strategy)
	{
		return this;
	}
}