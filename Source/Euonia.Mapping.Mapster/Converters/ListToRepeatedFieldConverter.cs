using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Mapster;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The converter used to convert <see cref="IEnumerable{TSource}"/> to <see cref="RepeatedField{TDestination}"/>.
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TDestination"></typeparam>
public class ListToRepeatedFieldConverter<TSource, TDestination> : IRegister
{
	/// <inheritdoc />
	public void Register(TypeAdapterConfig config)
    {
        config.ForType<IEnumerable<TSource>, RepeatedField<TDestination>>().MapToTargetWith((source, destination) => Convert(source, destination));
    }

    private static RepeatedField<TDestination> Convert(IEnumerable<TSource> source, RepeatedField<TDestination> destination)
    {
        if (source == null)
        {
            return null;
        }

        destination ??= new RepeatedField<TDestination>();

        if (typeof(TDestination) != typeof(Timestamp))
        {
            foreach (var item in source)
            {
                destination.Add(item.Adapt<TDestination>());
            }
        }
        else
        {
            foreach (var item in source)
            {
                // obviously we haven't performed the mapping for the item yet
                // since AutoMapper didn't recognise the list conversion
                // so we need to map the item here and then add it to the new
                // collection

                if (item is not DateTime time)
                {
                    continue;
                }

                var value = Timestamp.FromDateTime(DateTime.SpecifyKind(time, DateTimeKind.Utc));
                if (value is TDestination dest)
                {
                    destination.Add(dest);
                }
            }
        }

        return destination;
    }
}