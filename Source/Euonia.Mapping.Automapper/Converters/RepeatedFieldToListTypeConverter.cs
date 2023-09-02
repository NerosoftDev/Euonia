using AutoMapper;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The converter used to convert <see cref="RepeatedField{T}"/> to <see cref="IEnumerable{T}"/>.
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TDestination"></typeparam>
public class RepeatedFieldToListTypeConverter<TSource, TDestination> : ITypeConverter<RepeatedField<TSource>, IEnumerable<TDestination>>
{
	/// <inheritdoc />
	public IEnumerable<TDestination> Convert(RepeatedField<TSource> source, IEnumerable<TDestination> destination, ResolutionContext context)
    {
        if (source == null)
        {
            return null;
        }

        if (typeof(TSource) != typeof(Timestamp))
        {
            return source.Select(item => context.Mapper.Map<TDestination>(item)).ToList();
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