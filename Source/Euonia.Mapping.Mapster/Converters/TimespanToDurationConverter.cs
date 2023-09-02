using Google.Protobuf.WellKnownTypes;
using Mapster;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The converter used to convert <see cref="TimeSpan"/> to <see cref="Duration"/>.
/// </summary>
public class TimespanToDurationConverter : IRegister
{
	/// <inheritdoc />
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