using Mapster;

namespace Nerosoft.Euonia.Mapping;

public class ByteArrayToInt64Converter : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<byte[], long>().MapWith(source => BitConverter.ToInt64(source));
    }
}