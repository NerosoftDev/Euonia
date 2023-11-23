namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public class BytesMessageDataConverter : IMessageDataConverter<byte[]>
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public async Task<byte[]> Convert(Stream stream, CancellationToken cancellationToken)
	{
		using var ms = new MemoryStream();

		await stream.CopyToAsync(ms, 4096, cancellationToken).ConfigureAwait(false);

		return ms.ToArray();
	}
}