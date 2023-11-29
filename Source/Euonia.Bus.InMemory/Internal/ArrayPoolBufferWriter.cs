using System.Buffers;
using System.Runtime.CompilerServices;

namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// A simple buffer writer implementation using pooled arrays.
/// </summary>
/// <typeparam name="T">The type of items to store in the list.</typeparam>
/// <remarks>
/// This type is a <see langword="ref"/> <see langword="struct"/> to avoid the object allocation and to
/// enable the pattern-based <see cref="IDisposable"/> support. We aren't worried with consumers not
/// using this type correctly since it's private and only accessible within the parent type.
/// </remarks>
internal ref struct ArrayPoolBufferWriter<T>
{
	/// <summary>
	/// The default buffer size to use to expand empty arrays.
	/// </summary>
	private const int DEFAULT_INITIAL_BUFFER_SIZE = 128;

	/// <summary>
	/// The underlying <typeparamref name="T"/> array.
	/// </summary>
	private T[] _array;

	/// <summary>
	/// The span mapping to <see cref="_array"/>.
	/// </summary>
	/// <remarks>All writes are done through this to avoid covariance checks.</remarks>
	private Span<T> _span;

	/// <summary>
	/// The starting offset within <see cref="_array"/>.
	/// </summary>
	private int _index;

	/// <summary>
	/// Creates a new instance of the <see cref="ArrayPoolBufferWriter{T}"/> struct.
	/// </summary>
	/// <returns>A new <see cref="ArrayPoolBufferWriter{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ArrayPoolBufferWriter<T> Create()
	{
		ArrayPoolBufferWriter<T> instance;

		instance._span = instance._array = ArrayPool<T>.Shared.Rent(DEFAULT_INITIAL_BUFFER_SIZE);
		instance._index = 0;

		return instance;
	}

	/// <summary>
	/// Gets a <see cref="ReadOnlySpan{T}"/> with the current items.
	/// </summary>
	public ReadOnlySpan<T> Span
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _span[.._index];
	}

	/// <summary>
	/// Adds a new item to the current collection.
	/// </summary>
	/// <param name="item">The item to add.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(T item)
	{
		var span = _span;
		var index = _index;

		if ((uint)index < (uint)span.Length)
		{
			span[index] = item;

			_index = index + 1;
		}
		else
		{
			ResizeBufferAndAdd(item);
		}
	}

	/// <summary>
	/// Resets the underlying array and the stored items.
	/// </summary>
	public void Reset()
	{
		Array.Clear(_array, 0, _index);

		_index = 0;
	}

	/// <summary>
	/// Resizes <see cref="_array"/> when there is no space left for new items, then adds one
	/// </summary>
	/// <param name="item">The item to add.</param>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ResizeBufferAndAdd(T item)
	{
		var rent = ArrayPool<T>.Shared.Rent(_index << 2);

		Array.Copy(_array, 0, rent, 0, _index);
		Array.Clear(_array, 0, _index);

		ArrayPool<T>.Shared.Return(_array);

		_span = _array = rent;

		_span[_index++] = item;
	}

	/// <inheritdoc cref="IDisposable.Dispose"/>
	public void Dispose()
	{
		Array.Clear(_array, 0, _index);

		ArrayPool<T>.Shared.Return(_array);
	}
}