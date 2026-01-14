using System.Security.Claims;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Nerosoft.Euonia.Claims;

/// <summary>
/// Represents a user's identity and provides typed accessors and helpers for common claim values.
/// </summary>
/// <remarks>
/// This is a light wrapper around <see cref="ClaimsPrincipal"/> that exposes common application-specific
/// claim values (subject id, username, code, tenant, roles) and convenience methods for querying claims
/// and roles. The wrapper does not modify the underlying <see cref="ClaimsPrincipal"/>.
/// </remarks>
public class UserPrincipal
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UserPrincipal"/> class with the supplied claims principal.
	/// </summary>
	/// <param name="claims">The <see cref="ClaimsPrincipal"/> that contains the user's claims. May be <c>null</c>, in which
	/// case most accessors will return <c>null</c> or <c>false</c> as appropriate.</param>
	public UserPrincipal(ClaimsPrincipal claims)
	{
		Claims = claims;
	}

	/// <summary>
	/// Gets the underlying <see cref="ClaimsPrincipal"/> instance.
	/// </summary>
	public ClaimsPrincipal Claims { get; }

	/// <summary>
	/// Gets the user's identifier (subject).
	/// </summary>
	/// <remarks>
	/// This property attempts to find a subject identifier in the following order:
	/// <list type="bullet">
	/// <item><description><see cref="UserClaimTypes.Subject"/></description></item>
	/// <item><description><see cref="ClaimTypes.NameIdentifier"/></description></item>
	/// </list>
	/// If no matching claim is found, the property returns <c>null</c>.
	/// </remarks>
	public string UserId
	{
		get
		{
			return Claims?.Identity?.AuthenticationType switch
			{
				null or "Anonymous" => null,
				// For JWT/Bearer, prefer the 'sub' claim
				"Jwt" or "Bearer" => Claims.FindFirst(UserClaimTypes.Subject)?.Value,
				// For Windows auth, prefer the NameIdentifier claim
				"Windows" => Claims.FindFirst(ClaimTypes.NameIdentifier)?.Value,
				// For Cookie auth, prefer the NameIdentifier claim
				"Cookies" or "Cookie" => Claims.FindFirst(ClaimTypes.NameIdentifier)?.Value,
				_ => null
			};
		}
	}

	/// <summary>
	/// Gets the user's display name.
	/// </summary>
	/// <value>
	/// The value of the claim identified by <see cref="UserClaimTypes.Name"/>, or <c>null</c> if the claim is missing
	/// or if the underlying <see cref="ClaimsPrincipal"/> is <c>null</c>.
	/// </value>
	public string Username
	{
		get
		{
			return Claims.Identity?.AuthenticationType switch
			{
				null or "Anonymous" => null,
				// For JWT/Bearer, prefer the 'name' claim
				"Jwt" or "Bearer" => Claims?.FindFirst(UserClaimTypes.Name)?.Value,
				// For Windows auth, prefer the Name claim
				"Windows" => Claims?.FindFirst(ClaimTypes.Name)?.Value,
				// For Cookie auth, prefer the Name claim
				"Cookies" or "Cookie" => Claims?.FindFirst(ClaimTypes.Name)?.Value,
				_ => null
			};
		}
	}

	/// <summary>
	/// Gets the user's code.
	/// </summary>
	/// <value>
	/// The value of the claim identified by <see cref="UserClaimTypes.Code"/>, or <c>null</c> if the claim is missing
	/// or if the underlying <see cref="ClaimsPrincipal"/> is <c>null</c>.
	/// </value>
	public string Code => Claims?.FindFirst(UserClaimTypes.Code)?.Value;

	/// <summary>
	/// Gets the tenant identifier associated with the user.
	/// </summary>
	/// <value>
	/// The value of the claim identified by <see cref="UserClaimTypes.Tenant"/>. May be <c>null</c> if the claim or the
	/// underlying <see cref="ClaimsPrincipal"/> is not present.
	/// </value>
	public string Tenant => Claims.FindFirst(UserClaimTypes.Tenant)?.Value;

	/// <summary>
	/// Gets the user's roles as a sequence of role names.
	/// </summary>
	/// <remarks>
	/// This property selects the values of all claims with type <see cref="UserClaimTypes.Role"/>.
	/// The returned sequence may be <c>null</c> if the underlying <see cref="ClaimsPrincipal"/> is <c>null</c>.
	/// Callers should tolerate an empty sequence or <c>null</c> depending on usage.
	/// </remarks>
	/// <value>An <see cref="IEnumerable{T}"/> of role names (claim values) or <c>null</c>.</value>
	public IEnumerable<string> Roles => Claims?.FindAll(UserClaimTypes.Role).Select(t => t.Value);

	/// <summary>
	/// Gets a value that indicates whether the user is authenticated.
	/// </summary>
	/// <value><c>true</c> when the underlying <see cref="ClaimsPrincipal"/>'s identity is authenticated; otherwise <c>false</c>.
	/// If the underlying principal or identity is <c>null</c>, returns <c>false</c>.</value>
	public bool IsAuthenticated => Claims?.Identity?.IsAuthenticated ?? false;

	/// <summary>
	/// Finds the first claim with the specified claim type.
	/// </summary>
	/// <param name="claimType">The claim type to search for.</param>
	/// <returns>
	/// The first matching <see cref="Claim"/> if found; otherwise <c>null</c>.
	/// </returns>
	public Claim FindClaim(string claimType)
	{
		return Claims.FindFirst(claimType);
	}

	/// <summary>
	/// Finds all claims that match the specified claim type.
	/// </summary>
	/// <param name="claimType">The claim type to search for.</param>
	/// <returns>
	/// An array containing all matching <see cref="Claim"/> instances. If no claims match, an empty array is returned.
	/// </returns>
	public Claim[] FindClaims(string claimType)
	{
		return Claims.FindAll(claimType).ToArray();
	}

	/// <summary>
	/// Returns all claims held by the underlying <see cref="ClaimsPrincipal"/>.
	/// </summary>
	/// <returns>
	/// An array containing all <see cref="Claim"/> instances from the underlying principal. If the principal has no claims,
	/// an empty array is returned.
	/// </returns>
	public Claim[] GetAllClaims()
	{
		return Claims.Claims.ToArray();
	}

	/// <summary>
	/// Determines whether the current user is in the specified role.
	/// </summary>
	/// <param name="role">The role name to check.</param>
	/// <returns>
	/// <c>true</c> if the user is authenticated and is in the specified role; otherwise <c>false</c>.
	/// </returns>
	public bool IsInRole(string role)
	{
		return IsAuthenticated && Claims.IsInRole(role);
	}

	/// <summary>
	/// Determines whether the current user is in any of the specified roles.
	/// </summary>
	/// <param name="roles">A sequence of role names to check.</param>
	/// <returns>
	/// <c>true</c> if the user is authenticated and is a member of at least one of the specified roles; otherwise <c>false</c>.
	/// If <paramref name="roles"/> is <c>null</c> or empty, <c>false</c> is returned.
	/// </returns>
	public bool IsInRoles(IEnumerable<string> roles)
	{
		return IsAuthenticated && roles.Any(IsInRole);
	}

	/// <summary>
	/// Determines whether the current user is in any of the roles specified by a single delimited string.
	/// </summary>
	/// <param name="role">A delimited string containing role names (for example: "Admin,User").</param>
	/// <param name="separator">The string used to separate roles in <paramref name="role"/>. Defaults to <c>","</c>.</param>
	/// <returns>
	/// <c>true</c> if the user is authenticated and is a member of at least one parsed role; otherwise <c>false</c>.
	/// </returns>
	public bool IsInRoles(string role, string separator = ",")
	{
#if NET5_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(role);
#else
		ArgumentAssert.ThrowIfNull(role, nameof(role));
#endif

		var roles = role.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		return IsInRoles(roles);
	}
}