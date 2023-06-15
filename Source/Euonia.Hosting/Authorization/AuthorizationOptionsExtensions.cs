namespace Microsoft.AspNetCore.Authorization;

/// <summary>
/// Extensions methods for AuthorizationOptions
/// </summary>
public static class AuthorizationOptionsExtensions
{
    /// <summary>
    /// Adds an authorization policy that checks for the existence of one or more scope claims
    /// </summary>
    /// <param name="options"></param>
    /// <param name="policyName"></param>
    /// <param name="scopes"></param>
    /// <returns></returns>
    public static AuthorizationOptions AddScopePolicy(this AuthorizationOptions options, string policyName, params string[] scopes)
    {
        options.AddPolicy(policyName, p =>
        {
            p.RequireAuthenticatedUser();
            p.RequireScope(scopes);
        });

        return options;
    }
}