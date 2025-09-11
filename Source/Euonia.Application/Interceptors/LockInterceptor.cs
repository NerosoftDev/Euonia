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
			if (string.IsNullOrEmpty(token))
			{
				token = $"{invocation.Method.DeclaringType?.FullName}.{invocation.Method.Name}";
			}

			var semaphoreSlim = LockInterceptorSemaphoreSlim.GetLock(token);

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
/// 
/// </summary>
internal class LockInterceptorSemaphoreSlim
{
	private static readonly Dictionary<string, SemaphoreSlim> _locks = new();

	public static SemaphoreSlim GetLock(string key)
	{
		lock (_locks)
		{
			if (!_locks.TryGetValue(key, out var semaphoreSlim))
			{
				semaphoreSlim = new SemaphoreSlim(1, 1);
				_locks[key] = semaphoreSlim;
			}

			return semaphoreSlim;
		}
	}
}