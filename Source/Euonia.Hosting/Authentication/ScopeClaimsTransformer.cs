using System.Security.Claims;

namespace Microsoft.AspNetCore.Authentication;

/// <summary>
/// Claims transformer to transform scope claims from space separated to separate claims
/// </summary>
public class ScopeClaimsTransformer : IClaimsTransformation
{
    /// <inheritdoc />
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        return Task.FromResult(principal.NormalizeScopeClaims());
    }
}