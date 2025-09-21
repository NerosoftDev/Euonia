using System.Reflection;
using Castle.DynamicProxy;

namespace Nerosoft.Euonia.Application;

/// <summary>
/// Specifies a lock interceptor.
/// </summary>
public class LockInterceptor : IInterceptor
{
	/// <inheritdoc />
	public void Intercept(IInvocation invocation)
	{
		var type = invocation.Method.GetCustomAttribute<LockAttribute>()
		           ?? invocation.Method.DeclaringType?.GetCustomAttribute<LockAttribute>();

		if (type != null)
		{
			var token = type.Token;
			var maximumCount = type.MaximumCount;
			if (string.IsNullOrEmpty(token))
			{
				token = $"{invocation.Method.DeclaringType?.FullName}.{invocation.Method.Name}";
			}

			var semaphoreSlim = LockInterceptorSemaphoreSlim.GetOrCreateLock(token, maximumCount);

			semaphoreSlim.Wait(type.Timeout);
			try
			{
				invocation.Proceed();
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}
		else
		{
			invocation.Proceed();
		}
	}
}

/// <summary>
/// The lock interceptor using SemaphoreSlim.
/// </summary>
internal static class LockInterceptorSemaphoreSlim
{
	private static readonly Dictionary<string, SemaphoreSlim> _locks = new();

	/// <summary>
	/// Gets the lock associated with the specified key.
	/// </summary>
	/// <param name="key"></param>
	/// <param name="maximumCount"></param>
	/// <returns></returns>
	public static SemaphoreSlim GetOrCreateLock(string key, int maximumCount = 1)
	{
		lock (_locks)
		{
			if (_locks.TryGetValue(key, out var semaphoreSlim))
			{
				return semaphoreSlim;
			}

			semaphoreSlim = new SemaphoreSlim(maximumCount, maximumCount);
			_locks[key] = semaphoreSlim;

			return semaphoreSlim;
		}
	}
}