namespace Microsoft.AspNetCore.Authentication;

/// <summary>
/// Constants for IdentityServer authentication.
/// </summary>
public class IdentityServerAuthenticationDefaults
{
    /// <summary>
    /// The authentication scheme
    /// </summary>
    public const string AuthenticationScheme = "Bearer";

    /// <summary>
    /// Value of the JWT typ header (IdentityServer4 v3+ sets this by default)
    /// </summary>
    public const string JwtAccessTokenTyp = "at+jwt";

    internal const string IntrospectionAuthenticationScheme = "IdentityServerAuthenticationIntrospection";
    internal const string JwtAuthenticationScheme = "IdentityServerAuthenticationJwt";
    internal const string TokenItemsKey = "idsrv4:tokenvalidation:token";
    internal const string EffectiveSchemeKey = "idsrv4:tokenvalidation:effective:";
}