using System.Text.Json;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Serializer for message using System.Text.Json.
/// </summary>
public class SystemTextJsonSerializer : IMessageSerializer
{
	/// <inheritdoc />
	public async Task<byte[]> SerializeAsync<T>(T message, CancellationToken cancellationToken = default)
	{
		await using var stream = new MemoryStream();
		await JsonSerializer.SerializeAsync(stream, message, cancellationToken: cancellationToken);
		return stream.ToArray();
	}

	/// <inheritdoc />
	public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
	{
		return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
	}

	/// <inheritdoc />
	public async Task<T> DeserializeAsync<T>(byte[] bytes, CancellationToken cancellationToken = default)
	{
		await using var stream = new MemoryStream(bytes);
		return await DeserializeAsync<T>(stream, cancellationToken);
	}

	public T Deserialize<T>(byte[] bytes)
	{
		return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(bytes));
	}

	public T Deserialize<T>(string json)
	{
		return JsonSerializer.Deserialize<T>(json);
	}

	public T Deserialize<T>(Stream stream)
	{
		return JsonSerializer.Deserialize<T>(stream);
	}

	public string Serialize<T>(T obj)
	{
		return JsonSerializer.Serialize(obj);
	}

	public byte[] SerializeToBytes<T>(T obj)
	{
		return Encoding.UTF8.GetBytes(Serialize(obj));
	}
}