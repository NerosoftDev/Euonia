namespace Microsoft.AspNetCore.Authentication;

/// <summary>
/// Supported token types
/// </summary>
public enum SupportedTokens
{
    /// <summary>
    /// JWTs and reference tokens
    /// </summary>
    Both,

    /// <summary>
    /// JWTs only
    /// </summary>
    Jwt,

    /// <summary>
    /// Reference tokens only
    /// </summary>
    Reference
}