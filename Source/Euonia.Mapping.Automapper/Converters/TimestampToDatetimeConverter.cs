using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Nerosoft.Euonia.Mapping;

/// <inheritdoc />
public class TimestampToDatetimeValueConverter : IValueConverter<Timestamp, DateTime?>
{
    /// <inheritdoc />
    public DateTime? Convert(Timestamp sourceMember, ResolutionContext context)
    {
        if (sourceMember == null)
        {
            return null;
        }

        var time = sourceMember.ToDateTime();
        return time;
    }
}

/// <inheritdoc />
public class TimestampToNotnullDatetimeTypeConverter : ITypeConverter<Timestamp, DateTime>
{
    /// <inheritdoc />
    public DateTime Convert(Timestamp source, DateTime destination, ResolutionContext context)
    {
        if (source == null)
        {
            return default;
        }

        var time = source.ToDateTime();
        return time;
    }
}

/// <inheritdoc />
public class TimestampToNullableDatetimeTypeConverter : ITypeConverter<Timestamp, DateTime?>
{
    /// <inheritdoc />
    public DateTime? Convert(Timestamp source, DateTime? destination, ResolutionContext context)
    {
        if (source == null)
        {
            return null;
        }

        var time = source.ToDateTime();
        return time;
    }
}