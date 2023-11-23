using System.Text.Json;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Serializer for message using System.Text.Json.
/// </summary>
public class SystemTextJsonSerializer : IMessageSerializer
{
	/// <inheritdoc />
	public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
	{
		return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
	}
}