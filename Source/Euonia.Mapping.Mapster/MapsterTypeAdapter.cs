using MapsterMapper;

namespace Nerosoft.Euonia.Mapping;

public class MapsterTypeAdapter : ITypeAdapter
{
    private readonly IMapper _mapper;

    public MapsterTypeAdapter(IMapper mapper)
    {
        _mapper = mapper;
    }

    public TDestination Adapt<TSource, TDestination>(TSource source)
        where TSource : class
        where TDestination : class
    {
        return _mapper.Map<TSource, TDestination>(source);
    }

    public TDestination Adapt<TSource, TDestination>(TSource source, TDestination destination)
        where TSource : class
        where TDestination : class
    {
        return _mapper.Map(source, destination);
    }

    public TDestination Adapt<TDestination>(object source) where TDestination : class
    {
        return _mapper.Map<TDestination>(source);
    }

    public object Adapt(object source, Type destinationType)
    {
        return _mapper.Map(source, source.GetType(), destinationType);
    }

    public TDestination Adapt<TDestination>(object source, TDestination destination)
        where TDestination : class
    {
        return _mapper.Map(source, destination);
    }
}