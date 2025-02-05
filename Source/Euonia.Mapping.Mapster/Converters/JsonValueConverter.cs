using Mapster;
using Newtonsoft.Json;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TDest"></typeparam>
public class JsonValueConverter<TDest> : IRegister
{
	/// <inheritdoc />
	public void Register(TypeAdapterConfig config)
	{
		config.ForType<string, TDest>().MapWith(source => JsonConvert.DeserializeObject<TDest>(source));
	}
}