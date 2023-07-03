using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Mapster;

namespace Nerosoft.Euonia.Mapping;

public class RepeatedFieldToListConverter<TSource, TDestination> : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<RepeatedField<TSource>, IEnumerable<TDestination>>()
              .MapToTargetWith((source, destination) => Convert(source, destination));
    }

    private static IEnumerable<TDestination> Convert(RepeatedField<TSource> source, IEnumerable<TDestination> destination)
    {
        if (source == null)
        {
            return null;
        }

        if (typeof(TSource) != typeof(Timestamp))
        {
            return source.Select(item => item.Adapt<TDestination>()).ToList();
        }

        destination ??= new List<TDestination>();

        foreach (var item in source)
        {
            if (item is not Timestamp timestamp)
            {
                continue;
            }

            var value = timestamp.ToDateTime();
            if (value is TDestination dest)
            {
                (destination as List<TDestination>)?.Add(dest);
            }
        }

        return destination;
    }
}