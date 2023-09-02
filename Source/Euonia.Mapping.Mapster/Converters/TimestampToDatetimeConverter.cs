using Google.Protobuf.WellKnownTypes;
using Mapster;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The converter used to convert <see cref="Timestamp"/> to <see cref="DateTime"/>.
/// </summary>
public class TimestampToDatetimeConverter : IRegister
{
	/// <inheritdoc />
	public void Register(TypeAdapterConfig config)
    {
        config.ForType<Timestamp, DateTime?>().MapWith(source => Convert(source));
        config.ForType<Timestamp, DateTime>().MapWith(source => Convert(source, default));
    }

    private static DateTime? Convert(Timestamp sourceMember)
    {
        return sourceMember?.ToDateTime();
    }

    private static DateTime Convert(Timestamp sourceMember, DateTime defaultValue)
    {
        return sourceMember?.ToDateTime() ?? defaultValue;
    }
}