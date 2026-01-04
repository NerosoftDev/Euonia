#if NET8_0_OR_GREATER
namespace System;

/// <summary>
/// Provides helper methods to find a value produced by prioritized factory functions.
/// </summary>
/// <remarks>
/// The finder methods accept priority queues of factory functions (synchronous or asynchronous)
/// and evaluate factories in the queue's dequeue order until a produced value satisfies the given predicate.
/// If no value satisfies the predicate, the provided default value is returned (wrapped in a Task for async variants).
/// </remarks>
public static class PriorityValueFinder
{
	/// <summary>
	/// Finds the first value produced by the prioritized factories that satisfies the specified predicate.
	/// </summary>
	/// <typeparam name="TValue">The type of value produced by the factories.</typeparam>
	/// <param name="queue">A <see cref="PriorityQueue{TElement,TPriority}"/> containing factory functions returning <typeparamref name="TValue"/>.
	/// Factories are evaluated in the queue's dequeue order.</param>
	/// <param name="predicate">A function used to test each produced value. The first value that returns <c>true</c> is returned.</param>
	/// <param name="defaultValue">The value to return if no produced value satisfies <paramref name="predicate"/>. Defaults to the default of <typeparamref name="TValue"/>.</param>
	/// <returns>The first value that satisfies <paramref name="predicate"/>, or <paramref name="defaultValue"/> if none do.</returns>
	public static TValue Find<TValue>(PriorityQueue<Func<TValue>, int> queue, Func<TValue, bool> predicate, TValue defaultValue = default)
	{
		while (queue.Count > 0)
		{
			if (!queue.TryDequeue(out var factory, out _))
			{
				continue;
			}

			var value = factory();
			if (predicate(value))
			{
				return value;
			}
		}

		return defaultValue;
	}

	/// <summary>
	/// Creates a priority queue using the provided <paramref name="factory"/>, then finds the first produced value that satisfies <paramref name="predicate"/>.
	/// </summary>
	/// <typeparam name="TValue">The type of value produced by the factories.</typeparam>
	/// <param name="factory">An action that populates a <see cref="PriorityQueue{TElement,TPriority}"/> with factory functions returning <typeparamref name="TValue"/>.</param>
	/// <param name="predicate">A function used to test each produced value.</param>
	/// <param name="defaultValue">The value to return if no produced value satisfies <paramref name="predicate"/>.</param>
	/// <returns>The first value that satisfies <paramref name="predicate"/>, or <paramref name="defaultValue"/> if none do.</returns>
	public static TValue Find<TValue>(Action<PriorityQueue<Func<TValue>, int>> factory, Func<TValue, bool> predicate, TValue defaultValue = default)
	{
		var queue = new PriorityQueue<Func<TValue>, int>();
		factory(queue);
		var value = Find(queue, predicate, defaultValue);
		queue.Clear();
		return value;
	}

	/// <summary>
	/// Finds the first value produced by the prioritized asynchronous factories that satisfies the specified predicate.
	/// </summary>
	/// <typeparam name="TValue">The type of value produced by the factories.</typeparam>
	/// <param name="queue">A <see cref="PriorityQueue{TElement,TPriority}"/> containing factory functions returning <see cref="Task{TResult}"/> of <typeparamref name="TValue"/>.</param>
	/// <param name="predicate">A function used to test each produced value. The first value that returns <c>true</c> is returned.</param>
	/// <param name="defaultValue">The value to return if no produced value satisfies <paramref name="predicate"/>. Defaults to the default of <typeparamref name="TValue"/>.</param>
	/// <returns>A completed <see cref="Task{TResult}"/> containing the first value that satisfies <paramref name="predicate"/>, or <paramref name="defaultValue"/> if none do.</returns>
	/// <remarks>
	/// This method executes asynchronous factory functions synchronously by blocking on their returned tasks
	/// using <c>GetAwaiter().GetResult()</c>. Consumers should be aware this may block the calling thread.
	/// </remarks>
	public static async Task<TValue> FindAsync<TValue>(PriorityQueue<Func<Task<TValue>>, int> queue, Func<TValue, bool> predicate, TValue defaultValue = default)
	{
		while (queue.Count > 0)
		{
			if (!queue.TryDequeue(out var factory, out _))
			{
				continue;
			}

			var value = await factory();
			if (predicate(value))
			{
				return value;
			}
		}

		return await Task.FromResult(defaultValue);
	}

	/// <summary>
	/// Creates a priority queue of asynchronous factories using the provided <paramref name="factory"/>, then finds the first produced value that satisfies <paramref name="predicate"/>.
	/// </summary>
	/// <typeparam name="TValue">The type of value produced by the factories.</typeparam>
	/// <param name="factory">An action that populates a <see cref="PriorityQueue{TElement,TPriority}"/> with factory functions returning <see cref="Task{TResult}"/> of <typeparamref name="TValue"/>.</param>
	/// <param name="predicate">A function used to test each produced value.</param>
	/// <param name="defaultValue">The value to return if no produced value satisfies <paramref name="predicate"/>.</param>
	/// <returns>A completed <see cref="Task{TResult}"/> containing the first value that satisfies <paramref name="predicate"/>, or <paramref name="defaultValue"/> if none do.</returns>
	public static async Task<TValue> FindAsync<TValue>(Action<PriorityQueue<Func<Task<TValue>>, int>> factory, Func<TValue, bool> predicate, TValue defaultValue = default)
	{
		var queue = new PriorityQueue<Func<Task<TValue>>, int>();
		factory(queue);
		var value = await FindAsync(queue, predicate, defaultValue);
		queue.Clear();
		return value;
	}
}
#endif