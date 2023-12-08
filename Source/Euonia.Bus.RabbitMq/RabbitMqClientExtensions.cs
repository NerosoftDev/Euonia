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
	public static bool ExistsQueue(this IModel channel, string queueName)
	{
		//var queueDeclareOk = channel.QueueDeclarePassive(queueName);
		//if (queueDeclareOk.MessageCount > 0)
		//{
		//	throw new InvalidOperationException($"Queue '{queueName}' is not empty.");
		//}

		try
		{
			var queueDeclare = channel.QueueDeclarePassive(queueName);

			return queueDeclare?.ConsumerCount > 0;
		}
		catch (OperationInterruptedException exception)
		{
			if (exception.ShutdownReason.ReplyCode == 404)
			{
				return false;
				//throw new InvalidOperationException("No consumer found for the channel.", exception);
			}

			throw;
		}
	}

	public static QueueDeclareOk DeclareQueuePassively(this IModel channel, string queueName)
	{
		try
		{
			return channel.QueueDeclarePassive(queueName);
		}
		catch (OperationInterruptedException exception)
		{
			if (exception.ShutdownReason.ReplyCode == 404)
			{
				return null;
				//throw new InvalidOperationException("No consumer found for the channel.", exception);
			}

			throw;
		}
	}
}