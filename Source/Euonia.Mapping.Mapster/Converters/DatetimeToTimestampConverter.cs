using Google.Protobuf.WellKnownTypes;
using Mapster;

namespace Nerosoft.Euonia.Mapping;

public class DatetimeToTimestampConverter : IRegister
{
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