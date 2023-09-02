using MapsterMapper;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The <see cref="ITypeAdapter"/> implementation use Mapster.
/// </summary>
public class MapsterTypeAdapter : ITypeAdapter
{
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapsterTypeAdapter"/> class.
    /// </summary>
    /// <param name="mapper"></param>
    public MapsterTypeAdapter(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <inheritdoc />
    public TDestination Adapt<TSource, TDestination>(TSource source)
        where TSource : class
        where TDestination : class
    {
        return _mapper.Map<TSource, TDestination>(source);
    }

    /// <inheritdoc />
    public TDestination Adapt<TSource, TDestination>(TSource source, TDestination destination)
        where TSource : class
        where TDestination : class
    {
        return _mapper.Map(source, destination);
    }

    /// <inheritdoc />
    public TDestination Adapt<TDestination>(object source) where TDestination : class
    {
        return _mapper.Map<TDestination>(source);
    }

    /// <inheritdoc />
    public object Adapt(object source, Type destinationType)
    {
        return _mapper.Map(source, source.GetType(), destinationType);
    }

    /// <inheritdoc />
    public TDestination Adapt<TDestination>(object source, TDestination destination)
        where TDestination : class
    {
        return _mapper.Map(source, destination);
    }
}