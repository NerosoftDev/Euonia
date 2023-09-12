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
	/// <summary>
	/// 
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="message"></param>
	/// <param name="args"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	public static bool Ensure(Func<bool> condition, string message, params object[] args)
	{
		if (!condition())
		{
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, message, args));
		}

		return true;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="message"></param>
	/// <param name="args"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	public static bool Ensure(bool condition, string message, params object[] args)
	{
		if (!condition)
		{
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, message, args));
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
	/// Ensures the specified string value is not null, empty, or whitespace.
	/// </summary>
	/// <param name="value">The string value to check.</param>
	/// <param name="parameter">The name of the parameter being checked.</param>
	/// <param name="failsAction">The action to perform if the check fails.</param>
	public static void EnsureNotNullOrWhiteSpace(string value, [NotNull] string parameter, Action<string> failsAction)
	{
		if (value.IsNullOrWhiteSpace())
		{
			failsAction(parameter);
		}
	}

	/// <summary>
	/// Ensures that the specified string value is not null or empty.
	/// </summary>
	/// <param name="value">The value to check.</param>
	/// <param name="parameter">The name of the parameter being checked.</param>
	/// <returns>The same string value that was passed in if it is not null or empty.</returns>
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
	/// Checks if the input string is null or empty and calls the failsAction delegate if it is.
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
	/// Ensure the given string value is matched the regular expression pattern.
	/// </summary>
	/// <param name="value">The input value.</param>
	/// <param name="parameter">The parameter name of the given value.</param>
	/// <param name="pattern">The regular expression pattern.</param>
	/// <param name="options">The regular expression options.</param>
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
	/// Ensure the given string value is matched the regular expression pattern.
	/// </summary>
	/// <param name="value">The input value.</param>
	/// <param name="parameter">The parameter name of the given value.</param>
	/// <param name="pattern">The regular expression pattern.</param>
	/// <param name="failsAction">Callback function while the given is not matches.</param>
	/// <param name="options">The regular expression options.</param>
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
			throw new ArgumentException(string.Format(Resources.ERROR_PARAMETER_CANNOT_NULL_OR_EMPTY, parameter), parameter);
		}

		return collection;
	}

	/// <summary>
	/// Ensure that the given type is assignable to a specified type base class.
	/// </summary>
	/// <typeparam name="TBaseType">The base type the given type should be assignable to.</typeparam>
	/// <param name="type">The type to check if it is assignable to the specified base type.</param>
	/// <param name="parameter">The name of the parameter passed to the method.</param>
	/// <returns>The original input type, if it is assignable to TBaseType, or else throws an ArgumentException.</returns>
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
	/// Ensures that the length of the given input string is within the specified range.
	/// </summary>
	/// <param name="value">The input string to check.</param>
	/// <param name="parameter">The name of the parameter that corresponds to the input string.</param>
	/// <param name="maxLength">The maximum length of the input string.</param>
	/// <param name="minLength">The minimum length of the input string. Defaults to 0.</param>
	/// <returns>The input string.</returns>
	/// <exception cref="ArgumentException"></exception>
	public static string EnsureLengthInRange(string value, [NotNull] string parameter, int maxLength, int minLength = 0)
	{
		if (minLength > 0)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException(string.Format(Resources.ERROR_PARAMETER_CANNOT_NULL_OR_EMPTY, parameter), parameter);
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