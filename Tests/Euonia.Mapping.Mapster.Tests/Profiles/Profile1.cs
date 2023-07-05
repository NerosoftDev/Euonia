using Mapster;
using Nerosoft.Euonia.Mapping.Tests.Models;

namespace Nerosoft.Euonia.Mapping.Tests.Profiles;

public class Profile1 : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<SourceObject1, DestinationObject1>();
    }
}