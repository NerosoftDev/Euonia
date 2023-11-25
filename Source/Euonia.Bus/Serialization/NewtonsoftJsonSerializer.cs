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
		await writer.FlushAsync();

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

	public T Deserialize<T>(byte[] bytes)
	{
		return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
	}

	public T Deserialize<T>(string json)
	{
		return JsonConvert.DeserializeObject<T>(json);
	}

	public T Deserialize<T>(Stream stream)
	{
		using var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
		using var jsonReader = new JsonTextReader(reader);

		var value = JsonSerializer.Create().Deserialize<T>(jsonReader);

		return value;
	}

	public string Serialize<T>(T obj)
	{
		return JsonConvert.SerializeObject(obj);
	}

	public byte[] SerializeToBytes<T>(T obj)
	{
		return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
	}
}