using System.Security.Authentication;
using System.Security.Claims;
using Nerosoft.Euonia.Claims;

/// <summary>
/// Extension methods for <see cref="UserPrincipal"/> and <see cref="ClaimsPrincipal"/>.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static Guid GetUserIdOfGuid(this UserPrincipal user)
    {
        if (!user.IsAuthenticated || string.IsNullOrWhiteSpace(user.UserId))
        {
            throw new AuthenticationException();
        }

        if (!(Guid.TryParse(user.UserId, out var userId)))
        {
            throw new FormatException();
        }

        return userId;
    }

    /// <summary>
    /// Gets user id of <see cref="long"/>.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="AuthenticationException"></exception>
    /// <exception cref="FormatException"></exception>
    public static long GetUserIdOfInt64(this UserPrincipal user)
    {
        if (!user.IsAuthenticated || string.IsNullOrWhiteSpace(user.UserId))
        {
            throw new AuthenticationException();
        }

        if (!(long.TryParse(user.UserId, out var userId)))
        {
            throw new FormatException();
        }

        return userId;
    }

    /// <summary>
    /// Gets user id of int32.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="AuthenticationException"></exception>
    /// <exception cref="FormatException"></exception>
    public static int GetUserIdOfInt32(this UserPrincipal user)
    {
        if (!user.IsAuthenticated || string.IsNullOrWhiteSpace(user.UserId))
        {
            throw new AuthenticationException();
        }

        if (!(int.TryParse(user.UserId, out var userId)))
        {
            throw new FormatException();
        }

        return userId;
    }

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="user"></param>
    /// <exception cref="AuthenticationException"></exception>
    public static void EnsureAuthenticated(this UserPrincipal user)
    {
        if (!user.IsAuthenticated && !string.IsNullOrWhiteSpace(user.Username) && !string.IsNullOrWhiteSpace(user.UserId))
        {
            throw new AuthenticationException();
        }
    }

    /// <summary>
    /// Ensure user is in roles.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="roles"></param>
    /// <param name="message"></param>
    /// <exception cref="AuthenticationException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public static void EnsureInRoles(this UserPrincipal user, IEnumerable<string> roles, string message)
    {
        user.EnsureAuthenticated();

        if (!user.IsInRoles(roles))
        {
            throw new UnauthorizedAccessException(message);
        }
    }

    /// <summary>
    /// Ensure user is in roles.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="roles"></param>
    /// <param name="successCallback"></param>
    /// <param name="failureCallback"></param>
    /// <exception cref="AuthenticationException"></exception>
    public static void EnsureInRoles(this UserPrincipal user, IEnumerable<string> roles, Action successCallback = null, Action failureCallback = null)
    {
        user.EnsureAuthenticated();

        if (user.IsInRoles(roles))
        {
            successCallback?.Invoke();
        }
        else
        {
            failureCallback?.Invoke();
        }
    }

    /// <summary>
    /// Ensure user is in roles.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="roles"></param>
    /// <param name="successCallback"></param>
    /// <param name="failureCallback"></param>
    /// <returns></returns>
    /// <exception cref="AuthenticationException"></exception>
    public static Task EnsureInRolesAsync(this UserPrincipal user, IEnumerable<string> roles, Func<Task> successCallback = null, Func<Task> failureCallback = null)
    {
        user.EnsureAuthenticated();

        if (user.IsInRoles(roles))
        {
            return successCallback?.Invoke();
        }
        else
        {
            return failureCallback?.Invoke();
        }
    }

    /// <summary>
    /// Logic for normalizing scope claims to separate claim types
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static ClaimsPrincipal NormalizeScopeClaims(this ClaimsPrincipal principal)
    {
        var identities = new List<ClaimsIdentity>();

        foreach (var id in principal.Identities)
        {
            var identity = new ClaimsIdentity(id.AuthenticationType, id.NameClaimType, id.RoleClaimType);

            foreach (var claim in id.Claims)
            {
                if (claim.Type == "scope")
                {
                    if (claim.Value.Contains(' '))
                    {
                        var scopes = claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        foreach (var scope in scopes)
                        {
                            identity.AddClaim(new Claim("scope", scope, claim.ValueType, claim.Issuer));
                        }
                    }
                    else
                    {
                        identity.AddClaim(claim);
                    }
                }
                else
                {
                    identity.AddClaim(claim);
                }
            }

            identities.Add(identity);
        }

        return new ClaimsPrincipal(identities);
    }
}