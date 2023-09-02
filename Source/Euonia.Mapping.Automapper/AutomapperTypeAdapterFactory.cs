using AutoMapper;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The <see cref="ITypeAdapterFactory"/> implementation use automapper.
/// </summary>
public class AutomapperTypeAdapterFactory : ITypeAdapterFactory
{
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutomapperTypeAdapterFactory"/> class.
    /// </summary>
    /// <param name="mapper"></param>
    public AutomapperTypeAdapterFactory(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <inheritdoc />
    public ITypeAdapter Create()
    {
        return new AutomapperTypeAdapter(_mapper);
    }
}