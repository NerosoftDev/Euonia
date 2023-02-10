using System.Security.Claims;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Nerosoft.Euonia.Claims;

/// <summary>
/// 
/// </summary>
public class UserPrincipal
{
    private readonly ClaimsPrincipal _claims;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="claims"></param>
    public UserPrincipal(ClaimsPrincipal claims)
    {
        _claims = claims;
    }

    /// <summary>
    /// Gets the user id.
    /// </summary>
    public string UserId
    {
        get
        {
            var claim = _claims.FindFirst(UserClaimTypes.Subject);
            claim ??= _claims.FindFirst(ClaimTypes.NameIdentifier);
            var subjectId = claim?.Value;
            return subjectId;
        }
    }

    /// <summary>
    /// Gets the user name.
    /// </summary>
    public string Username => _claims?.FindFirst(UserClaimTypes.Name)?.Value;

    /// <summary>
    /// Gets the user code.
    /// </summary>
    public string Code => _claims?.FindFirst(UserClaimTypes.Code)?.Value;

    /// <summary>
    /// Gets the user tenant id.
    /// </summary>
    public string Tenant => _claims.FindFirst(UserClaimTypes.Tenant)?.Value;

    /// <summary>
    /// Gets the user roles.
    /// </summary>
    public IEnumerable<string> Roles => _claims?.FindAll(UserClaimTypes.Role)?.Select(t => t.Value);

    /// <summary>
    /// Determines whether the user is authenticated or not.
    /// </summary>
    public bool IsAuthenticated => _claims?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="claimType"></param>
    /// <returns></returns>
    public Claim FindClaim(string claimType)
    {
        return _claims.FindFirst(claimType);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="claimType"></param>
    /// <returns></returns>
    public Claim[] FindClaims(string claimType)
    {
        return _claims.FindAll(claimType).ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Claim[] GetAllClaims()
    {
        return _claims.Claims.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public bool IsInRole(string role)
    {
        return IsAuthenticated && _claims.IsInRole(role);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="roles"></param>
    /// <returns></returns>
    public bool IsInRoles(IEnumerable<string> roles)
    {
        return IsAuthenticated && roles.Any(IsInRole);
    }
}