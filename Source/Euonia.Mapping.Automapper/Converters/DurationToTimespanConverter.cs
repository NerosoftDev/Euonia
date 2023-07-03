using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Nerosoft.Euonia.Mapping;

/// <inheritdoc />
public class DurationToTimespanValueConverter : IValueConverter<Duration, TimeSpan?>
{
    /// <inheritdoc />
    public TimeSpan? Convert(Duration sourceMember, ResolutionContext context)
    {
        return sourceMember?.ToTimeSpan();
    }
}

/// <inheritdoc />
public class DurationToNullableTimespanTypeConverter : ITypeConverter<Duration, TimeSpan?>
{
    /// <inheritdoc />
    public TimeSpan? Convert(Duration source, TimeSpan? destination, ResolutionContext context)
    {
        return source?.ToTimeSpan();
    }
}

/// <inheritdoc />
public class DurationToNotnullTimespanTypeConverter : ITypeConverter<Duration, TimeSpan>
{
    /// <inheritdoc />
    public TimeSpan Convert(Duration source, TimeSpan destination, ResolutionContext context)
    {
        return source?.ToTimeSpan() ?? default;
    }
}