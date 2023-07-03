using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

/// <summary>
/// Exposes static methods for check value.
/// </summary>
[DebuggerStepThrough]
public static class Check
{
    public static bool Ensure(Func<bool> condition, string message, params object[] args)
    {
        if (!condition())
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.InvariantCulture, message, args));
        }

        return true;
    }

    public static bool Ensure(bool condition, string message, params object[] args)
    {
        if (!condition)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.InvariantCulture, message, args));
        }

        return true;
    }

    /// <summary>
    /// Ensure the given value is match the condition.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="action"></param>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public static T Ensure<T>(T value, [NotNull] Func<T, bool> action, string message)
    {
        var result = action(value);
        if (result)
        {
            return value;
        }

        throw new ArgumentException(message, nameof(value));
    }

    /// <summary>
    /// Ensure the given value is match the condition.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="action"></param>
    /// <param name="failsAction"></param>
    /// <typeparam name="T"></typeparam>
    public static void Ensure<T>(T value, [NotNull] Func<T, bool> action, Action<T> failsAction)
    {
        var result = action(value);
        if (result)
        {
            return;
        }

        failsAction(value);
    }

    /// <summary>
    /// Ensure the given value is match the condition.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static CheckResult<T> Ensure<T>(T value, [NotNull] Func<T, bool> action)
    {
        var result = action(value);
        return new CheckResult<T>(value, result);
    }

    /// <summary>
    /// Ensure the given value is not null.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static T EnsureNotNull<T>(T value, [NotNull] string parameter)
    {
        if (value == null)
        {
            throw new ArgumentNullException(parameter);
        }

        return value;
    }

    /// <summary>
    /// Ensure the given value is not null.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static T EnsureNotNull<T>(T value, [NotNull] string parameter, string message)
    {
        if (value == null)
        {
            throw new ArgumentNullException(parameter, message);
        }

        return value;
    }

    /// <summary>
    /// Ensure the given string value is not null.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="maxLength"></param>
    /// <param name="minLength"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string EnsureNotNull(string value, [NotNull] string parameter, int maxLength = int.MaxValue, int minLength = 0)
    {
        if (value == null)
        {
            throw new ArgumentException($"{parameter} can not be null!", parameter);
        }

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"{parameter} length must be equal to or lower than {maxLength}!", parameter);
        }

        if (minLength > 0 && value.Length < minLength)
        {
            throw new ArgumentException($"{parameter} length must be equal to or bigger than {minLength}!", parameter);
        }

        return value;
    }

    /// <summary>
    /// Ensure the given value is not null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The given string value.</param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string EnsureNotNullOrWhiteSpace(string value, [NotNull] string parameter)
    {
        if (value.IsNullOrWhiteSpace())
        {
            throw new ArgumentException($"{parameter} can not be null, empty or white space!", parameter);
        }

        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="failsAction"></param>
    /// <returns></returns>
    public static void EnsureNotNullOrWhiteSpace(string value, [NotNull] string parameter, Action<string> failsAction)
    {
        if (value.IsNullOrWhiteSpace())
        {
            failsAction(parameter);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string EnsureNotNullOrEmpty(string value, [NotNull] string parameter)
    {
        if (value.IsNullOrEmpty())
        {
            throw new ArgumentException($"{parameter} can not be null or empty!", parameter);
        }

        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="failsAction"></param>
    /// <returns></returns>
    public static void EnsureNotNullOrEmpty(string value, [NotNull] string parameter, Action<string> failsAction)
    {
        if (value.IsNullOrEmpty())
        {
            failsAction(parameter);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="pattern"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string EnsureIsMatch(string value, [NotNull] string parameter, string pattern, RegexOptions options = RegexOptions.None)
    {
        if (!Regex.IsMatch(value, pattern, options))
        {
            throw new ArgumentException($"{parameter} is not match with {pattern}!", parameter);
        }

        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="pattern"></param>
    /// <param name="failsAction"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static void EnsureIsMatch(string value, [NotNull] string parameter, string pattern, Action<string> failsAction, RegexOptions options = RegexOptions.None)
    {
        if (!Regex.IsMatch(value, pattern, options))
        {
            failsAction(parameter);
        }
    }

    /// <summary>
    /// Ensure the <paramref name="collection"/> is not null or empty.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="parameter"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static ICollection<T> EnsureNotNullOrEmpty<T>(ICollection<T> collection, [NotNull] string parameter)
    {
        if (collection.IsNullOrEmpty())
        {
            throw new ArgumentException(parameter + " can not be null or empty!", parameter);
        }

        return collection;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameter"></param>
    /// <typeparam name="TBaseType"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Type EnsureAssignableTo<TBaseType>(Type type, [NotNull] string parameter)
    {
        EnsureNotNull(type, parameter);

        if (!type.IsAssignableTo<TBaseType>())
        {
            throw new ArgumentException($"{parameter} (type of {type.AssemblyQualifiedName}) should be assignable to the {typeof(TBaseType).GetFullNameWithAssemblyName()}!");
        }

        return type;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="maxLength"></param>
    /// <param name="minLength"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string EnsureLengthInRange(string value, [NotNull] string parameter, int maxLength, int minLength = 0)
    {
        if (minLength > 0)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(parameter + " can not be null or empty!", parameter);
            }

            if (value.Length < minLength)
            {
                throw new ArgumentException($"{parameter} length must be equal to or bigger than {minLength}!", parameter);
            }
        }

        if (value != null && value.Length > maxLength)
        {
            throw new ArgumentException($"{parameter} length must be equal to or lower than {maxLength}!", parameter);
        }

        return value;
    }
}