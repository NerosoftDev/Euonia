namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IMessageDataConverter<T>
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<T> Convert(Stream stream, CancellationToken cancellationToken);
}