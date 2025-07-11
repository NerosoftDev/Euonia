using System.Collections.Concurrent;

namespace System;

/// <summary>
/// Defines a pool for <see cref="StringBuilder"/> instances.
/// </summary>
internal class StringBuilderPool
{
	private readonly ConcurrentBag<StringBuilder> _builders = [];

	public StringBuilder Get() => _builders.TryTake(out var builder) ? builder : new StringBuilder();

	/// <summary>
	/// Returns a <see cref="StringBuilder"/> instance to the pool.
	/// </summary>
	/// <param name="builder"></param>
	public void Return(StringBuilder builder)
	{
		builder.Clear();
		_builders.Add(builder);
	}
}