using Mapster;
using Nerosoft.Euonia.Mapping.Tests.Models;

namespace Nerosoft.Euonia.Mapping.Tests.Profiles;

public class Profile2 : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<SourceObject2, DestinationObject2>()
              .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");
    }
}