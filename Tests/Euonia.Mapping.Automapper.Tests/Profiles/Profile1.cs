using AutoMapper;
using Nerosoft.Euonia.Mapping.Tests.Models;

namespace Nerosoft.Euonia.Mapping.Tests.Profiles;

public class Profile1 : Profile
{
    public Profile1()
    {
        CreateMap<SourceObject1, DestinationObject1>();
    }
}