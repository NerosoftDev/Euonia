using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Nerosoft.Euonia.Bus.RabbitMq;

internal static class RabbitMqClientExtensions
{
	/// <summary>
	/// Checks if the queue exists.
	/// </summary>
	/// <param name="channel"></param>
	/// <param name="queueName"></param>
	/// <returns></returns>
	public static async Task<bool> ExistsQueueAsync(this IChannel channel, string queueName)
	{
		//var queueDeclareOk = channel.QueueDeclarePassive(queueName);
		//if (queueDeclareOk.MessageCount > 0)
		//{
		//	throw new InvalidOperationException($"Queue '{queueName}' is not empty.");
		//}

		try
		{
			var queueDeclare = await channel.QueueDeclarePassiveAsync(queueName);

			return queueDeclare?.ConsumerCount > 0;
		}
		catch (OperationInterruptedException exception)
		{
			if (exception.ShutdownReason?.ReplyCode == 404)
			{
				return false;
				//throw new InvalidOperationException("No consumer found for the channel.", exception);
			}

			throw;
		}
	}

	public static async Task<QueueDeclareOk> DeclareQueuePassivelyAsync(this IChannel channel, string queueName)
	{
		try
		{
			return await channel.QueueDeclarePassiveAsync(queueName);
		}
		catch (OperationInterruptedException exception)
		{
			if (exception.ShutdownReason?.ReplyCode == 404)
			{
				return null;
				//throw new InvalidOperationException("No consumer found for the channel.", exception);
			}

			throw;
		}
	}

	public static async Task<string> ResponseQueueDeclareAsync(this IChannel channel, CancellationToken cancellationToken = default)
	{
		var queue = await channel.QueueDeclareAsync(cancellationToken: cancellationToken);
		return queue.QueueName;
	}
}