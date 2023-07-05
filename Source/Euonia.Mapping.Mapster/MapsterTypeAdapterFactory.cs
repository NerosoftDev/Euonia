using MapsterMapper;

namespace Nerosoft.Euonia.Mapping;

public class MapsterTypeAdapterFactory : ITypeAdapterFactory
{
    private readonly IMapper _mapper;

    public MapsterTypeAdapterFactory(IMapper mapper)
    {
        _mapper = mapper;
    }

    public ITypeAdapter Create()
    {
        return new MapsterTypeAdapter(_mapper);
    }
}