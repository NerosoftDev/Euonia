namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The message serializer interface.
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

	/// <summary>
	/// Deserializes json text to the specified type.
	/// </summary>
	/// <param name="json"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	T Deserialize<T>(string json);

	/// <summary>
	/// Deserializes the specified stream to the specified type.
	/// </summary>
	/// <param name="stream"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	T Deserialize<T>(Stream stream);

	/// <summary>
	/// Serializes the specified object to a JSON string.
	/// </summary>
	/// <param name="obj"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	string Serialize<T>(T obj);

	/// <summary>
	/// Serializes the specified object to a JSON byte array.
	/// </summary>
	/// <param name="obj"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	byte[] SerializeToByteArray<T>(T obj);

	/// <summary>
	/// Deserializes the json bytes to the specified type.
	/// </summary>
	/// <param name="bytes"></param>
	/// <param name="type"></param>
	/// <returns></returns>
	object Deserialize(byte[] bytes, Type type) => Deserialize(Encoding.UTF8.GetString(bytes), type);

	/// <summary>
	/// Deserializes the json text to the specified type.
	/// </summary>
	/// <param name="json"></param>
	/// <param name="type"></param>
	/// <returns></returns>
	object Deserialize(string json, Type type);
}