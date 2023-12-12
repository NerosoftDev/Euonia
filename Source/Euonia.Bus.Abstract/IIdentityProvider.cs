using System.Security.Principal;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Defines the identity provider.
/// </summary>
public interface IIdentityProvider
{
	/// <summary>
	/// Get the identity from authorization header.
	/// </summary>
	/// <param name="authorization"></param>
	/// <returns></returns>
	IPrincipal GetIdentity(string authorization);
}