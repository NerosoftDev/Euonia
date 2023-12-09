using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System;

/// <summary>
/// Internal polyfill for <see cref="System.ArgumentNullException"/>.
/// </summary>
public sealed class ArgumentAssert
{
	/// <summary>
	/// Throws an <see cref="System.ArgumentNullException"/> if <paramref name="argument"/> is <see langword="null"/>.
	/// </summary>
	/// <param name="argument">The reference type argument to validate as non-<see langword="null"/>.</param>
	/// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET5_0_OR_GREATER
	public static void ThrowIfNull([NotNull] object argument, [CallerArgumentExpression(nameof(argument))] string paramName = null)
#else
	public static void ThrowIfNull([NotNull] object argument, string paramName = null)
#endif
	{
		if (argument is null)
		{
			Throw(paramName);
		}
	}

	/// <summary>
	/// A specialized version for generic values.
	/// </summary>
	/// <typeparam name="T">The type of values to check.</typeparam>
	/// <remarks>
	/// This type is needed because if there had been a generic overload with a generic parameter, all calls
	/// would have just been bound by that by the compiler instead of the <see cref="object"/> overload.
	/// </remarks>
	public static class For<T>
	{
		/// <summary>
		/// Throws an <see cref="System.ArgumentNullException"/> if <paramref name="argument"/> is <see langword="null"/>.
		/// </summary>
		/// <param name="argument">The reference type argument to validate as non-<see langword="null"/>.</param>
		/// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET5_0_OR_GREATER
		public static void ThrowIfNull([NotNull] T argument, [CallerArgumentExpression(nameof(argument))] string paramName = null)
#else
		public static void ThrowIfNull([NotNull] T argument, string paramName = null)
#endif
		{
			if (argument is null)
			{
				Throw(paramName);
			}
		}
	}

	/// <summary>
	/// Throws an <see cref="ArgumentNullException"/>.
	/// </summary>
	/// <param name="paramName">The name of the parameter that failed validation.</param>
	[DoesNotReturn]
	private static void Throw(string paramName)
	{
		throw new ArgumentNullException(paramName);
	}
}