using Mapster;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The converter used to convert <see cref="byte"/> array to <see cref="long"/>.
/// </summary>
public class ByteArrayToInt64Converter : IRegister
{
	/// <inheritdoc />
	public void Register(TypeAdapterConfig config)
    {
        config.ForType<byte[], long>().MapWith(source => BitConverter.ToInt64(source));
    }
}