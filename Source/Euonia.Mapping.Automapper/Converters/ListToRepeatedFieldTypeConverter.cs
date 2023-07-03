using AutoMapper;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;

namespace Nerosoft.Euonia.Mapping;

public class ListToRepeatedFieldTypeConverter<TSource, TDestination> : ITypeConverter<IEnumerable<TSource>, RepeatedField<TDestination>>
{
    public RepeatedField<TDestination> Convert(IEnumerable<TSource> source, RepeatedField<TDestination> destination, ResolutionContext context)
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
                destination.Add(context.Mapper.Map<TDestination>(item));
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