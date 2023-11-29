namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class JsonMessageDataConverter<T> : IMessageDataConverter<T>
{
	private readonly IMessageSerializer _serializer;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="serializer"></param>
	public JsonMessageDataConverter(IMessageSerializer serializer)
	{
		_serializer = serializer;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public Task<T> Convert(Stream stream, CancellationToken cancellationToken)
	{
		return _serializer.DeserializeAsync<T>(stream, cancellationToken);
	}
}