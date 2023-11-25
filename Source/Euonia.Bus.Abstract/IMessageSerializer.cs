namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public interface IMessageSerializer
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	Task<byte[]> SerializeAsync<T>(T message, CancellationToken cancellationToken = default);

	/// <summary>
	/// Asynchronously reads a JSON value from the provided <see cref="Stream"/> and converts it to an instance of a type specified by a generic parameter.
	/// </summary>
	/// <param name="stream">The JSON data to parse.</param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default);

	/// <summary>
	/// Asynchronously reads a JSON value from the provided <see cref="byte"/> array and converts it to an instance of a type specified by a generic parameter.
	/// </summary>
	/// <param name="bytes"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	Task<T> DeserializeAsync<T>(byte[] bytes, CancellationToken cancellationToken = default);

	/// <summary>
	/// Deserializes the specified bytes.
	/// </summary>
	/// <param name="bytes"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	T Deserialize<T>(byte[] bytes);
	
	T Deserialize<T>(string json);
	
	T Deserialize<T>(Stream stream);
	
	string Serialize<T>(T obj);
	
	byte[] SerializeToBytes<T>(T obj);
}