using System.Security.Principal;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The request context accessor.
/// </summary>
/// <param name="authorization"></param>
/// <returns></returns>
public delegate IPrincipal IdentityAccessor(string authorization);