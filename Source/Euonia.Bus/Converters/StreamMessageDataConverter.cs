namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public class StreamMessageDataConverter : IMessageDataConverter<Stream>
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public Task<Stream> Convert(Stream stream, CancellationToken cancellationToken)
	{
		return Task.FromResult(stream);
	}
}