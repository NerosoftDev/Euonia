using AutoMapper;
using Nerosoft.Euonia.Mapping.Tests.Models;

namespace Nerosoft.Euonia.Mapping.Tests;

public class Profile3 : Profile
{
	public Profile3()
	{
		CreateMap<SourceObject3, DestinationObject3>()
			.ForMember(dest => dest.FullName, options => options.MapFrom<FullNameValueResolver<SourceObject3,DestinationObject3>>());
	}
}