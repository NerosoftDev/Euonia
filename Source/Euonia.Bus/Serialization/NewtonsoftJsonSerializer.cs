using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Serializer for message using Newtonsoft.Json.
/// </summary>
public class NewtonsoftJsonSerializer : IMessageSerializer
{
	private readonly JsonSerializerSettings _settings;

	/// <summary>
	/// Initialize a new instance of <see cref="NewtonsoftJsonSerializer"/>
	/// </summary>
	/// <param name="options"></param>
	public NewtonsoftJsonSerializer(IOptions<JsonSerializerSettings> options)
	{
		_settings = options.Value;
	}

	/// <inheritdoc />
	public async Task<byte[]> SerializeAsync<T>(T message, CancellationToken cancellationToken = default)
	{
		await using var stream = new MemoryStream();
		await using var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true);
		await using var jsonWriter = new JsonTextWriter(writer);

		JsonSerializer.Create(_settings).Serialize(jsonWriter, message);

		await jsonWriter.FlushAsync(cancellationToken);

		await writer.FlushAsync(cancellationToken);

		return stream.ToArray();
	}

	/// <inheritdoc />
	public Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
	{
		using var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
		using var jsonReader = new JsonTextReader(reader);

		var value = JsonSerializer.Create(_settings).Deserialize<T>(jsonReader);

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
		return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes), _settings);
	}

	/// <inheritdoc />
	public T Deserialize<T>(string json)
	{
		return JsonConvert.DeserializeObject<T>(json, _settings);
	}

	/// <inheritdoc />
	public T Deserialize<T>(Stream stream)
	{
		using var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
		using var jsonReader = new JsonTextReader(reader);

		var value = JsonSerializer.Create(_settings).Deserialize<T>(jsonReader);

		return value;
	}

	/// <inheritdoc />
	public string Serialize<T>(T obj)
	{
		return JsonConvert.SerializeObject(obj, _settings);
	}

	/// <inheritdoc />
	public byte[] SerializeToByteArray<T>(T obj)
	{
		return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, _settings));
	}

	/// <inheritdoc />
	public object Deserialize(string json, Type type)
	{
		return JsonConvert.DeserializeObject(json, type, _settings);
	}
}