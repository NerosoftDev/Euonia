using MapsterMapper;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The <see cref="ITypeAdapterFactory"/> implementation use Mapster.
/// </summary>
public class MapsterTypeAdapterFactory : ITypeAdapterFactory
{
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapsterTypeAdapterFactory"/> class.
    /// </summary>
    /// <param name="mapper"></param>
    public MapsterTypeAdapterFactory(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <inheritdoc />
    public ITypeAdapter Create()
    {
        return new MapsterTypeAdapter(_mapper);
    }
}