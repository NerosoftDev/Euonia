using AutoMapper;
using Nerosoft.Euonia.Mapping.Tests.Models;

namespace Nerosoft.Euonia.Mapping.Tests.Profiles;

public class Profile2 : Profile
{
    public Profile2()
    {
        CreateMap<SourceObject2, DestinationObject2>()
            .ForMember(dest => dest.FullName, options => options.MapFrom(GetFullName));
    }

    private static string GetFullName(SourceObject2 source, DestinationObject2 dest, string arg3, ResolutionContext context)
    {
        return $"{source.FirstName} {source.LastName}";
    }
}