using Google.Protobuf.WellKnownTypes;
using Mapster;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The converter used to convert <see cref="TimeSpan"/> to <see cref="Duration"/>.
/// </summary>
public class DurationToTimespanConverter : IRegister
{
	/// <inheritdoc />
	public void Register(TypeAdapterConfig config)
    {
        config.ForType<Duration, TimeSpan>().MapWith(source => Convert(source, default));
        config.ForType<Duration, TimeSpan?>().MapWith(source => Convert(source));
    }

    private static TimeSpan? Convert(Duration sourceMember)
    {
        return sourceMember?.ToTimeSpan();
    }

    private static TimeSpan Convert(Duration sourceMember, TimeSpan defaultValue)
    {
        return sourceMember?.ToTimeSpan() ?? defaultValue;
    }
}