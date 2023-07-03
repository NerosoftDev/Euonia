using Google.Protobuf.WellKnownTypes;
using Mapster;

namespace Nerosoft.Euonia.Mapping;

public class TimespanToDurationConverter : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<object, Duration>()
              .MapWith(source => Convert(source));
    }

    private static Duration Convert<TSource>(TSource sourceMember)
    {
        return sourceMember switch
        {
            TimeSpan time => Duration.FromTimeSpan(time),
            null => null,
            _ => throw new InvalidCastException($"Type conversion from '{typeof(TSource).FullName}' to '{typeof(Duration).FullName}' is not supported.")
        };
    }
}