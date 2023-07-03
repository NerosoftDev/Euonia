namespace Microsoft.AspNetCore.Authentication;

public class JwtAuthenticationOptions
{
    public string Scheme { get; set; }

    /// <summary>
    /// Gets or sets the token issuers.
    /// </summary>
    public IEnumerable<string> Issuer { get; set; }

    public string SigningKey { get; set; }

    /// <summary>
    /// Gets or sets the authority url.
    /// </summary>
    public string Authority { get; set; }

    public bool RequireHttpsMetadata { get; set; }

    public string Audience { get; set; }

    public bool UsePolicy { get; set; } = true;
}