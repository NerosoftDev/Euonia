namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public interface IMessageSerializer
{
	/// <summary>
	/// Asynchronously reads a JSON value from the provided <see cref="Stream"/> and converts it to an instance of a type specified by a generic parameter.
	/// </summary>
	/// <param name="stream">The JSON data to parse.</param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default);
}