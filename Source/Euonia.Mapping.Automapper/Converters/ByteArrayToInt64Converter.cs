using AutoMapper;

namespace Nerosoft.Euonia.Mapping;

public class ByteArrayToInt64Converter : IValueConverter<byte[], long>
{
    public long Convert(byte[] sourceMember, ResolutionContext context)
    {
        return BitConverter.ToInt64(sourceMember);
    }
}