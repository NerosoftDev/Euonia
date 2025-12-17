using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Serializer for message using System.Text.Json.
/// </summary>
public class SystemTextJsonSerializer : IMessageSerializer
{
	private readonly JsonSerializerOptions _options;

	/// <summary>
	/// Initialize a new instance of <see cref="SystemTextJsonSerializer"/>
	/// </summary>
	/// <param name="options"></param>
	public SystemTextJsonSerializer(IOptions<JsonSerializerOptions> options)
	{
		_options = options.Value;
	}

	/// <inheritdoc />
	public async Task<byte[]> SerializeAsync<T>(T message, CancellationToken cancellationToken = default)
	{
		await using var stream = new MemoryStream();
		await JsonSerializer.SerializeAsync(stream, message, _options, cancellationToken: cancellationToken);
		return stream.ToArray();
	}

	/// <inheritdoc />
	public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
	{
		return await JsonSerializer.DeserializeAsync<T>(stream, _options, cancellationToken: cancellationToken);
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
		return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(bytes), _options);
	}

	/// <inheritdoc />
	public T Deserialize<T>(string json)
	{
		return JsonSerializer.Deserialize<T>(json, _options);
	}

	/// <inheritdoc />
	public T Deserialize<T>(Stream stream)
	{
		return JsonSerializer.Deserialize<T>(stream, _options);
	}

	/// <inheritdoc />
	public string Serialize<T>(T obj)
	{
		return JsonSerializer.Serialize(obj, _options);
	}

	/// <inheritdoc />
	public byte[] SerializeToByteArray<T>(T obj)
	{
		return Encoding.UTF8.GetBytes(Serialize(obj));
	}

	/// <inheritdoc />
	public object Deserialize(string json, Type type)
	{
		return JsonSerializer.Deserialize(json, type, _options);
	}
}