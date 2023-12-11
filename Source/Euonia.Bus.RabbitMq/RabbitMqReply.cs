namespace Nerosoft.Euonia.Bus.RabbitMq;

internal class RabbitMqReply<TResult>
{
	/// <summary>
	/// Gets or sets the result.
	/// </summary>
	public TResult Result { get; set; }

	/// <summary>
	/// Gets or sets the error.
	/// </summary>
	public Exception Error { get; set; }

	/// <summary>
	/// Gets a value indicating whether this message handing is success.
	/// </summary>
	public bool IsSuccess => Error == null;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="result"></param>
	/// <returns></returns>
	public static RabbitMqReply<TResult> Success(TResult result)
	{
		return new RabbitMqReply<TResult>
		{
			Result = result
		};
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="error"></param>
	/// <returns></returns>
	public static RabbitMqReply<TResult> Failure(Exception error)
	{
		return new RabbitMqReply<TResult>
		{
			Error = error
		};
	}
}