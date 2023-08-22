namespace Microsoft.AspNetCore.Authorization;

/// <summary>
/// Specifies the authorization policy to be used by the application.
/// </summary>
public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeRolesAttribute"/> class.
    /// </summary>
    /// <param name="roles"></param>
    public AuthorizeRolesAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
    }
}