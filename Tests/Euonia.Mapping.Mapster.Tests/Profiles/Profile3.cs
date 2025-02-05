using Mapster;
using Nerosoft.Euonia.Mapping.Tests.Models;

namespace Nerosoft.Euonia.Mapping.Tests.Profiles;

public class Profile3 : IRegister
{
	private readonly StringHelper _helper;

	public Profile3(StringHelper helper)
	{
		_helper = helper;
	}

	public void Register(TypeAdapterConfig config)
	{
		config.ForType<SourceObject3, DestinationObject3>()
		      .Map(dest => dest.FullName, src => _helper.Combine(src.FirstName, src.LastName));
	}
}