namespace Microsoft.AspNetCore.Authentication;

/// <summary>
/// The jwt authentication options.
/// </summary>
public class JwtAuthenticationOptions
{
	/// <summary>
	/// Gets or sets the scheme.
	/// </summary>
	public string Scheme { get; set; }

	/// <summary>
	/// Gets or sets the token issuers.
	/// </summary>
	public IEnumerable<string> Issuer { get; set; }

	/// <summary>
	/// Gets or sets the signing key.
	/// </summary>
	public string SigningKey { get; set; }

	/// <summary>
	/// Gets or sets the authority url.
	/// </summary>
	public string Authority { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the HTTPS metadata is required.
	/// </summary>
	public bool RequireHttpsMetadata { get; set; }

	/// <summary>
	/// Gets or sets the audience.
	/// </summary>
	public string Audience { get; set; }

	/// <summary>
	/// Gets or sets a value indicating if the policy should be used.
	/// </summary>
	public bool UsePolicy { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating if the issuer should be validated.
	/// </summary>
	public bool ValidateIssuer { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating if the audience should be validated.
	/// </summary>
	public bool ValidateAudience { get; set; } = true;
}