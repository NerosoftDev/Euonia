using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Nerosoft.Euonia.Mapping;

/// <inheritdoc />
public class DatetimeToTimestampValueConverter<TSource> : IValueConverter<TSource, Timestamp>
{
    /// <inheritdoc />
    public Timestamp Convert(TSource sourceMember, ResolutionContext context)
    {
        return sourceMember switch
        {
            DateTime time => Timestamp.FromDateTime(DateTime.SpecifyKind(time, DateTimeKind.Utc)),
            null => null,
            _ => null
        };
    }
}

/// <inheritdoc />
public class DatetimeToTimestampTypeConverter<TSource> : ITypeConverter<TSource, Timestamp>
{
    /// <inheritdoc />
    public Timestamp Convert(TSource source, Timestamp destination, ResolutionContext context)
    {
        return source switch
        {
            null => null,
            DateTime time => Timestamp.FromDateTime(DateTime.SpecifyKind(time, DateTimeKind.Utc)),
            _ => default
        };
    }
}