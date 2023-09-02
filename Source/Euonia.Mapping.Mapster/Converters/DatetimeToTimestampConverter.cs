using Google.Protobuf.WellKnownTypes;
using Mapster;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The converter used to convert <see cref="DateTime"/> to <see cref="Timestamp"/>.
/// </summary>
public class DatetimeToTimestampConverter : IRegister
{
	/// <inheritdoc />
	public void Register(TypeAdapterConfig config)
    {
        config.ForType<object, Timestamp>().MapWith(source => Convert(source));
    }

    private static Timestamp Convert<TSource>(TSource sourceMember)
    {
        return sourceMember switch
        {
            DateTime time => Timestamp.FromDateTime(DateTime.SpecifyKind(time, DateTimeKind.Utc)),
            null => null,
            _ => throw new InvalidCastException($"Type conversion from '{typeof(TSource).FullName}' to '{typeof(Timestamp).FullName}' is not supported.")
        };
    }
}