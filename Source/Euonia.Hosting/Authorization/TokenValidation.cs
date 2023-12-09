using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authorization;

/// <summary>
/// The jwt token validation class.
/// </summary>
public class TokenValidation
{
    /// <summary>
    /// Provides a forwarding func for JWT vs reference tokens (based on existence of dot in token)
    /// </summary>
    /// <param name="introspectionScheme">Scheme name of the introspection handler</param>
    /// <returns></returns>
    public static Func<HttpContext, string> ForwardReferenceToken(string introspectionScheme = "introspection")
    {
        string Select(HttpContext context)
        {
            var (scheme, credential) = GetSchemeAndCredential(context);
            if (scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) && !credential.Contains('.'))
            {
                return introspectionScheme;
            }

            return null;
        }

        return Select;
    }

    /// <summary>
    /// Extracts scheme and credential from Authorization header (if present)
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static (string, string) GetSchemeAndCredential(HttpContext context)
    {
        var header = context.Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrEmpty(header))
        {
            return ("", "");
        }

        var parts = header.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return ("", "");
        }

        return (parts[0], parts[1]);
    }
}