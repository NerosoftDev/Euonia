using Duende.AspNetCore.Authentication.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Authentication;

/// <summary>
/// Configures the internal options for IdentityServer authentication.
/// </summary>
internal class ConfigureInternalOptions :
    IConfigureNamedOptions<JwtBearerOptions>,
    IConfigureNamedOptions<OAuth2IntrospectionOptions>
{
    private readonly IdentityServerAuthenticationOptions _identityServerOptions;
    private readonly string _scheme;

    public ConfigureInternalOptions(IdentityServerAuthenticationOptions identityServerOptions, string scheme)
    {
        _identityServerOptions = identityServerOptions;
        _scheme = scheme;
    }

    /// <summary>
    /// Configures the JWT bearer options.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="options"></param>
    public void Configure(string name, JwtBearerOptions options)
    {
        if (name == _scheme + IdentityServerAuthenticationDefaults.JwtAuthenticationScheme &&
            _identityServerOptions.SupportsJwt)
        {
            _identityServerOptions.ConfigureJwtBearer(options);
        }
    }

    /// <summary>
    /// Configures the OAuth2 introspection options.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="options"></param>
    public void Configure(string name, OAuth2IntrospectionOptions options)
    {
        if (name == _scheme + IdentityServerAuthenticationDefaults.IntrospectionAuthenticationScheme &&
            _identityServerOptions.SupportsIntrospection)
        {
            _identityServerOptions.ConfigureIntrospection(options);
        }
    }

    /// <summary>
    /// Configures the JWT bearer options.
    /// </summary>
    /// <param name="options"></param>
    public void Configure(JwtBearerOptions options)
    {
    }

    /// <summary>
    /// Configures the OAuth2 introspection options.
    /// </summary>
    /// <param name="options"></param>
    public void Configure(OAuth2IntrospectionOptions options)
    {
    }
}