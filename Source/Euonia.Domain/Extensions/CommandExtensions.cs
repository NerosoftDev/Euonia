using System.ComponentModel;
using System.Security.Authentication;

namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The extensions for <see cref="ICommand"/>.
/// </summary>
/*
public static class CommandExtensions
{
    private const string HEADER_USER_ID = "$nerosoft:user.id";
    private const string HEADER_USER_NAME = "$nerosoft:user.name";
    private const string HEADER_USER_TENANT = "$nerosoft:user.tenant";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static string GetUsername(this ICommand command)
    {
        if (command.Metadata.TryGetValue(HEADER_USER_NAME, out var value))
        {
            return value as string;
        }

        return string.Empty;
    }

    /// <summary>
    /// Set current logged user name.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="username"></param>
    public static void SetUsername(this ICommand command, string username)
    {
        command.Metadata?.Set(HEADER_USER_NAME, username);
    }

    /// <summary>
    /// Get user id from command metadata.
    /// </summary>
    /// <param name="command"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    /// <exception cref="AuthenticationException"></exception>
    public static TValue GetUserId<TValue>(this ICommand command)
    {
        if (!command.Metadata.TryGetValue(HEADER_USER_ID, out var value))
        {
            throw new AuthenticationException();
        }

        return value switch
        {
            TValue id => id,
            string id => (TValue)TypeDescriptor.GetConverter(typeof(TValue)).ConvertFromString(id),
            _ => throw new InvalidCastException(),
        };
    }

    /// <summary>
    /// Gets user id from metadata and convert to Guid.
    /// </summary>
    /// <param name="command"></param>
    /// <returns>The user id.</returns>
    /// <exception cref="AuthenticationException">Throws if command metadata does not contains user id key.</exception>
    public static Guid GetUserIdOfGuid(this ICommand command)
    {
        if (!command.Metadata.TryGetValue(HEADER_USER_ID, out var value))
        {
            throw new AuthenticationException();
        }

        return value switch
        {
            Guid id => id,
            string id => Guid.Parse(id),
            _ => throw new InvalidCastException(),
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="AuthenticationException"></exception>
    public static long GetUserIdOfInt64(this ICommand command)
    {
        if (!command.Metadata.TryGetValue(HEADER_USER_ID, out var value))
        {
            throw new AuthenticationException();
        }

        return value switch
        {
            long id => id,
            string id => long.Parse(id),
            _ => throw new InvalidCastException(),
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="AuthenticationException"></exception>
    public static int GetUserIdOfInt32(this ICommand command)
    {
        if (!command.Metadata.TryGetValue(HEADER_USER_ID, out var value))
        {
            throw new AuthenticationException();
        }

        return value switch
        {
            int id => id,
            string id => int.Parse(id),
            _ => throw new InvalidCastException(),
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="id"></param>
    public static void SetUserId<TValue>(this ICommand command, TValue id)
    {
        command.Metadata?.Set(HEADER_USER_ID, id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="id"></param>
    public static void SetUserId(this ICommand command, long id)
    {
        command.SetUserId<long>(id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="id"></param>
    public static void SetUserId(this ICommand command, int id)
    {
        command.SetUserId<int>(id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="id"></param>
    public static void SetUserId(this ICommand command, Guid id)
    {
        command.SetUserId<Guid>(id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="id"></param>
    public static void SetUserId(this ICommand command, string id)
    {
        command.SetUserId<string>(id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static string GetTenant(this ICommand command)
    {
        if (command.Metadata.TryGetValue(HEADER_USER_TENANT, out var value))
        {
            return value as string;
        }

        return string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="tenant"></param>
    public static void SetTenant(this ICommand command, string tenant)
    {
        command.Metadata?.Set(HEADER_USER_TENANT, tenant);
    }
}
*/