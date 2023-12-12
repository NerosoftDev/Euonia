using System.Security.Principal;

namespace Nerosoft.Euonia.Bus;

internal class DefaultIdentityProvider : IIdentityProvider
{
	private readonly IdentityAccessor _accessor;

	public DefaultIdentityProvider(IdentityAccessor accessor)
	{
		_accessor = accessor;
	}

	public IPrincipal GetIdentity(string authorization)
	{
		return _accessor(authorization);
	}
}
