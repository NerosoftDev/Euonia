using Newtonsoft.Json;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Serializer for message using Newtonsoft.Json.
/// </summary>
public class NewtonsoftJsonSerializer : IMessageSerializer
{
	/// <inheritdoc />
	public async Task<byte[]> SerializeAsync<T>(T message, CancellationToken cancellationToken = default)
	{
		await using var stream = new MemoryStream();
		await using var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true);
		using var jsonWriter = new JsonTextWriter(writer);

		JsonSerializer.Create().Serialize(jsonWriter, message);

		await jsonWriter.FlushAsync(cancellationToken);

#if NET8_0_OR_GREATER
		await writer.FlushAsync(cancellationToken);
#else
		await writer.FlushAsync();
#endif

		return stream.ToArray();
	}

	/// <inheritdoc />
	public Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
	{
		using var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
		using var jsonReader = new JsonTextReader(reader);

		var value = JsonSerializer.Create().Deserialize<T>(jsonReader);

		return Task.FromResult(value);
	}

	/// <inheritdoc />
	public async Task<T> DeserializeAsync<T>(byte[] bytes, CancellationToken cancellationToken = default)
	{
		await using var stream = new MemoryStream(bytes);
		return await DeserializeAsync<T>(stream, cancellationToken);
	}

	/// <inheritdoc />
	public T Deserialize<T>(byte[] bytes)
	{
		return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
	}

	/// <inheritdoc />
	public T Deserialize<T>(string json)
	{
		return JsonConvert.DeserializeObject<T>(json);
	}

	/// <inheritdoc />
	public T Deserialize<T>(Stream stream)
	{
		using var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
		using var jsonReader = new JsonTextReader(reader);

		var value = JsonSerializer.Create().Deserialize<T>(jsonReader);

		return value;
	}

	/// <inheritdoc />
	public string Serialize<T>(T obj)
	{
		return JsonConvert.SerializeObject(obj);
	}

	/// <inheritdoc />
	public byte[] SerializeToByteArray<T>(T obj)
	{
		return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
	}

	/// <inheritdoc />
	public object Deserialize(string json, Type type)
	{
		return JsonConvert.DeserializeObject(json, type);
	}
}