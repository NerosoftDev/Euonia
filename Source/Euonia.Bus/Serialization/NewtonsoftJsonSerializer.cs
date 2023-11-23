using Newtonsoft.Json;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Serializer for message using Newtonsoft.Json.
/// </summary>
public class NewtonsoftJsonSerializer : IMessageSerializer
{
	public Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
	{
		using var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
		using var jsonReader = new JsonTextReader(reader);

		var value = JsonSerializer.Create().Deserialize<T>(jsonReader);

		return Task.FromResult(value);
	}
}