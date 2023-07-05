using AutoMapper;

namespace Nerosoft.Euonia.Mapping;

public class AutomapperTypeAdapterFactory : ITypeAdapterFactory
{
    private readonly IMapper _mapper;

    public AutomapperTypeAdapterFactory(IMapper mapper)
    {
        _mapper = mapper;
    }

    public ITypeAdapter Create()
    {
        return new AutomapperTypeAdapter(_mapper);
    }
}