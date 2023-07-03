using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Nerosoft.Euonia.Mapping;

/// <inheritdoc />
public class TimespanToDurationValueConverter : IValueConverter<object, Duration>
{
    /// <inheritdoc />
    public Duration Convert(object sourceMember, ResolutionContext context)
    {
        return sourceMember switch
        {
            TimeSpan time => Duration.FromTimeSpan(time),
            _ => null
        };
    }
}

/// <inheritdoc />
public class TimespanToDurationTypeConverter<TSource> : ITypeConverter<TSource, Duration>
{
    /// <inheritdoc />
    public Duration Convert(TSource source, Duration destination, ResolutionContext context)
    {
        return source switch
        {
            null => null,
            TimeSpan time => Duration.FromTimeSpan(time),
            _ => default
        };
    }
}