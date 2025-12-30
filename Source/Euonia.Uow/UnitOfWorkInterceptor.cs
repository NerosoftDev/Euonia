using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Threading;

namespace Nerosoft.Euonia.Uow;

/// <inheritdoc />
public class UnitOfWorkInterceptor : IInterceptor
{
	private readonly IServiceScopeFactory _factory;

	/// <summary>
	/// Initializes a new instance of the <see cref="UnitOfWorkInterceptor"/> class.
	/// </summary>
	/// <param name="factory"></param>
	public UnitOfWorkInterceptor(IServiceScopeFactory factory)
	{
		_factory = factory;
	}

	/// <inheritdoc />
	public void Intercept(IInvocation invocation)
	{
		if (UnitOfWorkHelper.IsUnitOfWorkMethod(invocation.Method, out var attribute))
		{
			invocation.Proceed();
		}
		else
		{
			using var scope = _factory.CreateScope();
			var manager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

			var isTransactional = FindValue<bool?>(queue =>
			{
				queue.Enqueue(() => attribute.IsTransactional, 1);
				queue.Enqueue(() => scope.ServiceProvider.GetService<IOptions<UnitOfWorkOptions>>()?.Value.IsTransactional, 2);
			}, t => t.HasValue) ?? false;

			var timeout = FindValue<TimeSpan?>(queue =>
			{
				queue.Enqueue(() => attribute.Timeout, 1);
				queue.Enqueue(() => scope.ServiceProvider.GetService<IOptions<UnitOfWorkOptions>>()?.Value.Timeout, 2);
			}, t => t.HasValue);

			using var uow = manager.Begin(isTransactional);
			invocation.Proceed();

			var cancellationToken = timeout.HasValue ? new CancellationTokenSource(timeout.Value).Token : CancellationToken.None;

			AsyncContext.Run(() => uow.CompleteAsync(cancellationToken));
		}
	}

	private static TValue FindValue<TValue>(PriorityQueue<Func<TValue>, int> queue, Func<TValue, bool> predicate)
	{
		if (queue.Count == 0)
		{
			throw new InvalidOperationException("The queue is empty.");
		}

		while (queue.TryDequeue(out var valueFactory, out _))
		{
			var value = valueFactory();
			if (predicate(value))
			{
				return value;
			}
		}

		return default;
	}

	private static TValue FindValue<TValue>(Action<PriorityQueue<Func<TValue>, int>> factory, Func<TValue, bool> predicate)
	{
		var queue = new PriorityQueue<Func<TValue>, int>();
		factory(queue);
		var value = FindValue(queue, predicate);
		queue.Clear();
		return value;
	}
}