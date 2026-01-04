namespace System;

/// <summary>
/// Provides utility methods for asserting exceptions in code execution.
/// </summary>
public static class ExceptionAssert
{
	/// <summary>
	/// Throws an exception to the specified type if the given condition is true.
	/// </summary>
	/// <typeparam name="TException">The type of exception to throw.</typeparam>
	/// <param name="condition">The condition to evaluate.</param>
	/// <param name="message">The message to include in the exception.</param>
	public static void ThrowIf<TException>(bool condition, string message)
		where TException : Exception
	{
		ThrowIf(condition, () => (TException)Activator.CreateInstance(typeof(TException), message)!);
	}

	/// <summary>
	/// Throws an exception of the specified type if the given condition is true.
	/// Uses a factory method to create the exception instance.
	/// </summary>
	/// <typeparam name="TException">The type of exception to throw.</typeparam>
	/// <param name="condition">The condition to evaluate.</param>
	/// <param name="exceptionFactory">A factory method to create the exception instance.</param>
	public static void ThrowIf<TException>(bool condition, Func<TException> exceptionFactory)
		where TException : Exception
	{
		if (!condition)
		{
			return;
		}

		var exception = exceptionFactory();
		throw exception;
	}

	/// <summary>
	/// Throws an exception of the specified type if the given condition is true.
	/// Assumes the exception has a parameterless constructor.
	/// </summary>
	/// <typeparam name="TException">The type of exception to throw.</typeparam>
	/// <param name="condition">The condition to evaluate.</param>
	public static void ThrowIf<TException>(bool condition)
		where TException : Exception, new()
	{
		ThrowIf(condition, () => new TException());
	}
}