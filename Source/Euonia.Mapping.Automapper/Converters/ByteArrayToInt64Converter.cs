using AutoMapper;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The converter used to convert <see cref="byte"/> array to <see cref="long"/>.
/// </summary>
public class ByteArrayToInt64Converter : IValueConverter<byte[], long>
{
	/// <inheritdoc />
	public long Convert(byte[] sourceMember, ResolutionContext context)
    {
        return BitConverter.ToInt64(sourceMember);
    }
}