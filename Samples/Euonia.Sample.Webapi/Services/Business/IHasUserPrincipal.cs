using Nerosoft.Euonia.Claims;

namespace Nerosoft.Euonia.Sample.Business;

public interface IHasUserPrincipal
{
	UserPrincipal Identity { get; }
}
