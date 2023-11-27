using System.Text.Json;
using System.Text.Json.Serialization;

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

	/// <inheritdoc />
	public T Deserialize<T>(byte[] bytes)
	{
		return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(bytes));
	}

	/// <inheritdoc />
	public T Deserialize<T>(string json)
	{
		return JsonSerializer.Deserialize<T>(json);
	}

	/// <inheritdoc />
	public T Deserialize<T>(Stream stream)
	{
		return JsonSerializer.Deserialize<T>(stream);
	}

	/// <inheritdoc />
	public string Serialize<T>(T obj)
	{
		return JsonSerializer.Serialize(obj);
	}

	/// <inheritdoc />
	public byte[] SerializeToByteArray<T>(T obj)
	{
		return Encoding.UTF8.GetBytes(Serialize(obj));
	}

	/// <inheritdoc />
	public object Deserialize(string json, Type type)
	{
		return JsonSerializer.Deserialize(json, type);
	}

	private static JsonSerializerOptions ConvertSettings(MessageSerializerSettings settings)
	{
		if (settings == null)
		{
			return default;
		}

		var options = new JsonSerializerOptions();
		switch (settings.ReferenceLoop)
		{
			case MessageSerializerSettings.ReferenceLoopStrategy.Ignore:
				options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
				break;
			case MessageSerializerSettings.ReferenceLoopStrategy.Preserve:
				options.ReferenceHandler = ReferenceHandler.Preserve;
				break;
			case MessageSerializerSettings.ReferenceLoopStrategy.Serialize:
			case null:
			default:
				break;
		}

		return options;
	}
}