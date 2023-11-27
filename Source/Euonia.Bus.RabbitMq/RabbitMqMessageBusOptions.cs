namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// The RabbitMQ based message bus options definition.
/// </summary>
public class RabbitMqMessageBusOptions
{
    /// <summary>
    /// Gets or sets the RabbitMQ connection string.
    /// <example>amqp://user:password@host:port</example>
    /// </summary>
    public string Connection { get; set; }

    /// <summary>
    /// Gets or sets the exchange name.
    /// </summary>
    public string ExchangeName { get; set; }

    /// <summary>
    /// Gets or sets the command queue name.
    /// </summary>
    public string QueueName { get; set; }

    /// <summary>
    /// Gets or sets the event queue name.
    /// </summary>
    public string EventQueueName { get; set; }

    /// <summary>
    /// Gets or sets the exchange type.
    /// Values: fanout, direct, headers, topic
    /// </summary>
    public string ExchangeType { get; set; } = RabbitMQ.Client.ExchangeType.Fanout;

    /// <summary>
    /// 
    /// </summary>
    public string RoutingKey { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool AutoAck { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public int MaxFailureRetries { get; set; } = 3;
}
