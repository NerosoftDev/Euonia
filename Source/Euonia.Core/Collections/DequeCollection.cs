namespace Nerosoft.Euonia.Collections;

/// <summary>
/// Class DequeCollection. This class cannot be inherited.
/// Implements the <see cref="IList{T}" />
/// Implements the <see cref="IReadOnlyList{T}" />
/// Implements the <see cref="IList" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="IList{T}" />
/// <seealso cref="IReadOnlyList{T}" />
/// <seealso cref="IList" />
public sealed class DequeCollection<T> : IList<T>, IReadOnlyList<T>, IList
{
	/// <summary>
	/// The default capacity.
	/// </summary>
	private const int DEFAULT_CAPACITY = 8;

	/// <summary>
	/// The circular _buffer that holds the view.
	/// </summary>
	private T[] _buffer;

	/// <summary>
	/// The offset into <see cref="_buffer" /> where the view begins.
	/// </summary>
	private int _offset;

	/// <summary>
	/// Initializes a new instance of the <see cref="DequeCollection{T}" /> class with the specified capacity.
	/// </summary>
	/// <param name="capacity">The initial capacity. Must be greater than <c>0</c>.</param>
	/// <exception cref="ArgumentOutOfRangeException">capacity - Capacity may not be negative.</exception>
	public DequeCollection(int capacity)
	{
		if (capacity < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(capacity), Resources.IDS_CAPACITY_MAY_NOT_BE_NEGATIVE);
		}

		_buffer = new T[capacity];
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DequeCollection{T}" /> class with the elements from the specified collection.
	/// </summary>
	/// <param name="collection">The collection. May not be <c>null</c>.</param>
	/// <exception cref="ArgumentNullException">collection</exception>
	public DequeCollection(IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException(nameof(collection));

		var source = collection.Reify();
		var count = source.Count;
		if (count > 0)
		{
			_buffer = new T[count];
			DoInsertRange(0, source);
		}
		else
		{
			_buffer = new T[DEFAULT_CAPACITY];
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DequeCollection{T}" /> class.
	/// </summary>
	public DequeCollection()
		: this(DEFAULT_CAPACITY)
	{
	}

	#region GenericListImplementations

	/// <summary>
	/// Gets a value indicating whether this list is read-only. This implementation always returns <c>false</c>.
	/// </summary>
	/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
	bool ICollection<T>.IsReadOnly => false;

	/// <summary>
	/// Gets or sets the item at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>T.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index in this list.</exception>
	/// <exception cref="T:System.NotSupportedException">This property is set and the list is read-only.</exception>
	public T this[int index]
	{
		get
		{
			CheckExistingIndexArgument(Count, index);
			return DoGetItem(index);
		}

		set
		{
			CheckExistingIndexArgument(Count, index);
			DoSetItem(index, value);
		}
	}

	/// <summary>
	/// Inserts an item to this list at the specified index.
	/// </summary>
	/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
	/// <param name="item">The object to insert into this list.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index in this list.</exception>
	/// <exception cref="T:System.NotSupportedException">This list is read-only.</exception>
	public void Insert(int index, T item)
	{
		CheckNewIndexArgument(Count, index);
		DoInsert(index, item);
	}

	/// <summary>
	/// Removes the item at the specified index.
	/// </summary>
	/// <param name="index">The zero-based index of the item to remove.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index in this list.</exception>
	/// <exception cref="T:System.NotSupportedException">This list is read-only.</exception>
	public void RemoveAt(int index)
	{
		CheckExistingIndexArgument(Count, index);
		DoRemoveAt(index);
	}

	/// <summary>
	/// Determines the index of a specific item in this list.
	/// </summary>
	/// <param name="item">The object to locate in this list.</param>
	/// <returns>The index of <paramref name="item" /> if found in this list; otherwise, -1.</returns>
	public int IndexOf(T item)
	{
		var comparer = EqualityComparer<T>.Default;
		var ret = 0;
		foreach (var sourceItem in this)
		{
			if (comparer.Equals(item, sourceItem))
				return ret;
			++ret;
		}

		return -1;
	}

	/// <summary>
	/// Adds an item to the end of this list.
	/// </summary>
	/// <param name="item">The object to add to this list.</param>
	/// <exception cref="T:System.NotSupportedException">This list is read-only.</exception>
	void ICollection<T>.Add(T item)
	{
		DoInsert(Count, item);
	}

	/// <summary>
	/// Determines whether this list contains a specific value.
	/// </summary>
	/// <param name="item">The object to locate in this list.</param>
	/// <returns>true if <paramref name="item" /> is found in this list; otherwise, false.</returns>
	bool ICollection<T>.Contains(T item)
	{
		var comparer = EqualityComparer<T>.Default;
		foreach (var entry in this)
		{
			if (comparer.Equals(item, entry))
				return true;
		}

		return false;
	}

	/// <summary>
	/// Copies the elements of this list to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
	/// </summary>
	/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from this slice. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
	/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
	/// <exception cref="ArgumentNullException">array</exception>
	/// <exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.</exception>
	/// <exception cref="T:System.ArgumentException"><paramref name="arrayIndex" /> is equal to or greater than the length of <paramref name="array" />.
	/// -or-
	/// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
	void ICollection<T>.CopyTo(T[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException(nameof(array));
		}

		var count = Count;
		CheckRangeArguments(array.Length, arrayIndex, count);
		CopyToArray(array, arrayIndex);
	}

	/// <summary>
	/// Copies the deque elements into an array. The resulting array always has all the deque elements contiguously.
	/// </summary>
	/// <param name="array">The destination array.</param>
	/// <param name="arrayIndex">The optional index in the destination array at which to begin writing.</param>
	/// <exception cref="ArgumentNullException">array</exception>
	private void CopyToArray(Array array, int arrayIndex = 0)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));

		if (IsSplit)
		{
			// The existing buffer is split, so we have to copy it in parts
			var length = Capacity - _offset;
			Array.Copy(_buffer, _offset, array, arrayIndex, length);
			Array.Copy(_buffer, 0, array, arrayIndex + length, Count - length);
		}
		else
		{
			// The existing buffer is whole
			Array.Copy(_buffer, _offset, array, arrayIndex, Count);
		}
	}

	/// <summary>
	/// Removes the first occurrence of a specific object from this list.
	/// </summary>
	/// <param name="item">The object to remove from this list.</param>
	/// <returns>true if <paramref name="item" /> was successfully removed from this list; otherwise, false. This method also returns false if <paramref name="item" /> is not found in this list.</returns>
	/// <exception cref="T:System.NotSupportedException">This list is read-only.</exception>
	public bool Remove(T item)
	{
		var index = IndexOf(item);
		if (index == -1)
			return false;

		DoRemoveAt(index);
		return true;
	}

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
	public IEnumerator<T> GetEnumerator()
	{
		var count = Count;
		for (var i = 0; i != count; ++i)
		{
			yield return DoGetItem(i);
		}
	}

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	#endregion

	#region ObjectListImplementations

	/// <summary>
	/// Determines whether the specified value is t.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns><c>true</c> if the specified value is t; otherwise, <c>false</c>.</returns>
	private static bool IsT(object value)
	{
		if (value is T)
			return true;
		if (value != null)
			return false;
		return default(T) == null;
	}

	/// <summary>
	/// Adds the specified value.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>System.Int32.</returns>
	/// <exception cref="ArgumentNullException">value - Value cannot be null.</exception>
	/// <exception cref="ArgumentException">Value is of incorrect type. - value</exception>
	int IList.Add(object value)
	{
		if (value == null && default(T) != null)
		{
			throw new ArgumentNullException(nameof(value), Resources.IDS_VALUE_CANNOT_BE_NULL);
		}

		if (!IsT(value))
		{
			throw new ArgumentException(Resources.IDS_VALUE_IS_OF_INCORRECT_TYPE, nameof(value));
		}

		AddToBack((T)value);
		return Count - 1;
	}

	/// <summary>
	/// Determines whether this instance contains the object.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns><c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.</returns>
	bool IList.Contains(object value)
	{
		return IsT(value) && ((ICollection<T>)this).Contains((T)value);
	}

	/// <summary>
	/// Indexes the of.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>System.Int32.</returns>
	int IList.IndexOf(object value)
	{
		return IsT(value) ? IndexOf((T)value) : -1;
	}

	/// <summary>
	/// Inserts the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <param name="value">The value.</param>
	/// <exception cref="ArgumentNullException">value - Value cannot be null.</exception>
	/// <exception cref="ArgumentException">Value is of incorrect type. - value</exception>
	void IList.Insert(int index, object value)
	{
		if (value == null && default(T) != null)
		{
			throw new ArgumentNullException(nameof(value), Resources.IDS_VALUE_CANNOT_BE_NULL);
		}

		if (!IsT(value))
		{
			throw new ArgumentException(Resources.IDS_VALUE_IS_OF_INCORRECT_TYPE, nameof(value));
		}

		Insert(index, (T)value);
	}

	/// <summary>
	/// Gets a value indicating whether this instance is fixed size.
	/// </summary>
	/// <value><c>true</c> if this instance is fixed size; otherwise, <c>false</c>.</value>
	bool IList.IsFixedSize => false;

	/// <summary>
	/// Gets a value indicating whether this instance is read only.
	/// </summary>
	/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
	bool IList.IsReadOnly => false;

	/// <summary>
	/// Removes the specified value.
	/// </summary>
	/// <param name="value">The value.</param>
	void IList.Remove(object value)
	{
		if (IsT(value))
		{
			Remove((T)value);
		}
	}

	/// <summary>
	/// Gets or sets the <see cref="System.Object"/> at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>System.Object.</returns>
	/// <exception cref="ArgumentNullException">value - Value cannot be null.</exception>
	/// <exception cref="ArgumentException">Value is of incorrect type. - value</exception>
	object IList.this[int index]
	{
		get => this[index];

		set
		{
			if (value == null && default(T) != null)
			{
				throw new ArgumentNullException(nameof(value), Resources.IDS_VALUE_CANNOT_BE_NULL);
			}

			if (!IsT(value))
			{
				throw new ArgumentException(Resources.IDS_VALUE_IS_OF_INCORRECT_TYPE, nameof(value));
			}

			this[index] = (T)value;
		}
	}

	/// <summary>
	/// Copies to.
	/// </summary>
	/// <param name="array">The array.</param>
	/// <param name="index">The index.</param>
	/// <exception cref="ArgumentNullException">array - Destination array cannot be null.</exception>
	/// <exception cref="ArgumentException">
	/// Destination array is of incorrect type. - array
	/// or
	/// Destination array must be single dimensional. - array
	/// </exception>
	void ICollection.CopyTo(Array array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException(nameof(array), Resources.IDS_DESTINATION_ARRAY_CANNOT_BE_NULL);
		}

		CheckRangeArguments(array.Length, index, Count);

		try
		{
			CopyToArray(array, index);
		}
		catch (ArrayTypeMismatchException ex)
		{
			throw new ArgumentException(Resources.IDS_DESTINATION_ARRAY_IS_OF_INCORRECT_TYPE, nameof(array), ex);
		}
		catch (RankException ex)
		{
			throw new ArgumentException(Resources.IDS_DESTINATION_ARRAY_MUST_BE_SINGLE_DIMENSIONAL, nameof(array), ex);
		}
	}

	/// <summary>
	/// Gets a value indicating whether this instance is synchronized.
	/// </summary>
	/// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
	bool ICollection.IsSynchronized => false;

	/// <summary>
	/// Gets the synchronize root.
	/// </summary>
	/// <value>The synchronize root.</value>
	object ICollection.SyncRoot => this;

	#endregion

	#region GenericListHelpers

	/// <summary>
	/// Checks the <paramref name="index" /> argument to see if it refers to a valid insertion point in a source of a given length.
	/// </summary>
	/// <param name="sourceLength">The length of the source. This parameter is not checked for validity.</param>
	/// <param name="index">The index into the source.</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index to an insertion point for the source.</exception>
	private static void CheckNewIndexArgument(int sourceLength, int index)
	{
		if (index < 0 || index > sourceLength)
		{
			throw new ArgumentOutOfRangeException(nameof(index), string.Format(Resources.IDS_INVALID_NEW_INDEX_FOR_SOURCE_LENGTH, index, sourceLength));
		}
	}

	/// <summary>
	/// Checks the <paramref name="index" /> argument to see if it refers to an existing element in a source of a given length.
	/// </summary>
	/// <param name="sourceLength">The length of the source. This parameter is not checked for validity.</param>
	/// <param name="index">The index into the source.</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index to an existing element for the source.</exception>
	private static void CheckExistingIndexArgument(int sourceLength, int index)
	{
		if (index < 0 || index >= sourceLength)
		{
			throw new ArgumentOutOfRangeException(nameof(index), string.Format(Resources.IDS_INVALID_EXISTING_INDEX_FOR_SOURCE_LENGTH, index, sourceLength));
		}
	}

	/// <summary>
	/// Checks the <paramref name="offset" /> and <paramref name="count" /> arguments for validity when applied to a source of a given length. Allows 0-element ranges, including a 0-element range at the end of the source.
	/// </summary>
	/// <param name="sourceLength">The length of the source. This parameter is not checked for validity.</param>
	/// <param name="offset">The index into source at which the range begins.</param>
	/// <param name="count">The number of elements in the range.</param>
	/// <exception cref="ArgumentOutOfRangeException">Either <paramref name="offset" /> or <paramref name="count" /> is less than 0.</exception>
	/// <exception cref="ArgumentException">The range [offset, offset + count) is not within the range [0, sourceLength).</exception>
	private static void CheckRangeArguments(int sourceLength, int offset, int count)
	{
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(offset), string.Format(Resources.IDS_INVALID_OFFSET, offset));
		}

		if (count < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(count), string.Format(Resources.IDS_INVALID_COUNT, count));
		}

		if (sourceLength - offset < count)
		{
			throw new ArgumentException(string.Format(Resources.IDS_INVALID_OFFSET_OR_COUNT_FOR_SOURCE_LENGTH, offset, count, sourceLength));
		}
	}

	#endregion

	/// <summary>
	/// Gets a value indicating whether this instance is empty.
	/// </summary>
	/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
	private bool IsEmpty => Count == 0;

	/// <summary>
	/// Gets a value indicating whether this instance is at full capacity.
	/// </summary>
	/// <value><c>true</c> if this instance is full; otherwise, <c>false</c>.</value>
	private bool IsFull => Count == Capacity;

	/// <summary>
	/// Gets a value indicating whether the buffer is "split" (meaning the beginning of the view is at a later index in <see cref="_buffer" /> than the end).
	/// </summary>
	/// <value><c>true</c> if this instance is split; otherwise, <c>false</c>.</value>
	// Overflow-safe version of "(offset + Count) > Capacity"
	private bool IsSplit => _offset > (Capacity - Count);

	/// <summary>
	/// Gets or sets the capacity for this deque. This value must always be greater than zero, and this property cannot be set to a value less than <see cref="Count" />.
	/// </summary>
	/// <value>The capacity.</value>
	/// <exception cref="ArgumentOutOfRangeException">value - Capacity cannot be set to a value less than Count</exception>
	/// <exception cref="InvalidOperationException"><c>Capacity</c> cannot be set to a value less than <see cref="Count" />.</exception>
	public int Capacity
	{
		get => _buffer.Length;

		set
		{
			if (value < Count)
			{
				throw new ArgumentOutOfRangeException(nameof(value), Resources.IDS_CAPACITY_CANNOT_BE_SET_TO_A_VALUE_LESS_THAN_COUNT);
			}

			if (value == _buffer.Length)
				return;

			// Create the new _buffer and copy our existing range.
			var newBuffer = new T[value];
			CopyToArray(newBuffer);

			// Set up to use the new _buffer.
			_buffer = newBuffer;
			_offset = 0;
		}
	}

	/// <summary>
	/// Gets the number of elements contained in this deque.
	/// </summary>
	/// <value>The count.</value>
	public int Count { get; private set; }

	/// <summary>
	/// Applies the offset to <paramref name="index" />, resulting in a buffer index.
	/// </summary>
	/// <param name="index">The deque index.</param>
	/// <returns>The buffer index.</returns>
	private int DequeIndexToBufferIndex(int index)
	{
		return (index + _offset) % Capacity;
	}

	/// <summary>
	/// Gets an element at the specified view index.
	/// </summary>
	/// <param name="index">The zero-based view index of the element to get. This index is guaranteed to be valid.</param>
	/// <returns>The element at the specified index.</returns>
	private T DoGetItem(int index)
	{
		return _buffer[DequeIndexToBufferIndex(index)];
	}

	/// <summary>
	/// Sets an element at the specified view index.
	/// </summary>
	/// <param name="index">The zero-based view index of the element to get. This index is guaranteed to be valid.</param>
	/// <param name="item">The element to store in the list.</param>
	private void DoSetItem(int index, T item)
	{
		_buffer[DequeIndexToBufferIndex(index)] = item;
	}

	/// <summary>
	/// Inserts an element at the specified view index.
	/// </summary>
	/// <param name="index">The zero-based view index at which the element should be inserted. This index is guaranteed to be valid.</param>
	/// <param name="item">The element to store in the list.</param>
	private void DoInsert(int index, T item)
	{
		EnsureCapacityForOneElement();

		if (index == 0)
		{
			DoAddToFront(item);
			return;
		}
		else if (index == Count)
		{
			DoAddToBack(item);
			return;
		}

		DoInsertRange(index, new[] { item });
	}

	/// <summary>
	/// Removes an element at the specified view index.
	/// </summary>
	/// <param name="index">The zero-based view index of the element to remove. This index is guaranteed to be valid.</param>
	private void DoRemoveAt(int index)
	{
		if (index == 0)
		{
			DoRemoveFromFront();
			return;
		}
		else if (index == Count - 1)
		{
			DoRemoveFromBack();
			return;
		}

		DoRemoveRange(index, 1);
	}

	/// <summary>
	/// Increments <see cref="_offset" /> by <paramref name="value" /> using modulo-<see cref="Capacity" /> arithmetic.
	/// </summary>
	/// <param name="value">The value by which to increase <see cref="_offset" />. May not be negative.</param>
	/// <returns>The value of <see cref="_offset" /> after it was incremented.</returns>
	private int PostIncrement(int value)
	{
		var ret = _offset;
		_offset += value;
		_offset %= Capacity;
		return ret;
	}

	/// <summary>
	/// Decrements <see cref="_offset" /> by <paramref name="value" /> using modulo-<see cref="Capacity" /> arithmetic.
	/// </summary>
	/// <param name="value">The value by which to reduce <see cref="_offset" />. May not be negative or greater than <see cref="Capacity" />.</param>
	/// <returns>The value of <see cref="_offset" /> before it was decremented.</returns>
	private int PreDecrement(int value)
	{
		_offset -= value;
		if (_offset < 0)
			_offset += Capacity;
		return _offset;
	}

	/// <summary>
	/// Inserts a single element to the back of the view. <see cref="IsFull" /> must be false when this method is called.
	/// </summary>
	/// <param name="value">The element to insert.</param>
	private void DoAddToBack(T value)
	{
		_buffer[DequeIndexToBufferIndex(Count)] = value;
		++Count;
	}

	/// <summary>
	/// Inserts a single element to the front of the view. <see cref="IsFull" /> must be false when this method is called.
	/// </summary>
	/// <param name="value">The element to insert.</param>
	private void DoAddToFront(T value)
	{
		_buffer[PreDecrement(1)] = value;
		++Count;
	}

	/// <summary>
	/// Removes and returns the last element in the view. <see cref="IsEmpty" /> must be false when this method is called.
	/// </summary>
	/// <returns>The former last element.</returns>
	private T DoRemoveFromBack()
	{
		var ret = _buffer[DequeIndexToBufferIndex(Count - 1)];
		--Count;
		return ret;
	}

	/// <summary>
	/// Removes and returns the first element in the view. <see cref="IsEmpty" /> must be false when this method is called.
	/// </summary>
	/// <returns>The former first element.</returns>
	private T DoRemoveFromFront()
	{
		--Count;
		return _buffer[PostIncrement(1)];
	}

	/// <summary>
	/// Inserts a range of elements into the view.
	/// </summary>
	/// <param name="index">The index into the view at which the elements are to be inserted.</param>
	/// <param name="collection">The elements to insert. The sum of <c>collection.Count</c> and <see cref="Count" /> must be less than or equal to <see cref="Capacity" />.</param>
	private void DoInsertRange(int index, IReadOnlyCollection<T> collection)
	{
		var collectionCount = collection.Count;
		// Make room in the existing list
		if (index < Count / 2)
		{
			// Inserting into the first half of the list

			// Move lower items down: [0, index) -> [Capacity - collectionCount, Capacity - collectionCount + index)
			// This clears out the low "index" number of items, moving them "collectionCount" places down;
			//   after rotation, there will be a "collectionCount"-sized hole at "index".
			var copyCount = index;
			var writeIndex = Capacity - collectionCount;
			for (var j = 0; j != copyCount; ++j)
				_buffer[DequeIndexToBufferIndex(writeIndex + j)] = _buffer[DequeIndexToBufferIndex(j)];

			// Rotate to the new view
			PreDecrement(collectionCount);
		}
		else
		{
			// Inserting into the second half of the list

			// Move higher items up: [index, count) -> [index + collectionCount, collectionCount + count)
			var copyCount = Count - index;
			var writeIndex = index + collectionCount;
			for (var j = copyCount - 1; j != -1; --j)
				_buffer[DequeIndexToBufferIndex(writeIndex + j)] = _buffer[DequeIndexToBufferIndex(index + j)];
		}

		// Copy new items into place
		var i = index;
		foreach (var item in collection)
		{
			_buffer[DequeIndexToBufferIndex(i)] = item;
			++i;
		}

		// Adjust valid count
		Count += collectionCount;
	}

	/// <summary>
	/// Removes a range of elements from the view.
	/// </summary>
	/// <param name="index">The index into the view at which the range begins.</param>
	/// <param name="collectionCount">The number of elements in the range. This must be greater than 0 and less than or equal to <see cref="Count" />.</param>
	private void DoRemoveRange(int index, int collectionCount)
	{
		if (index == 0)
		{
			// Removing from the beginning: rotate to the new view
			PostIncrement(collectionCount);
			Count -= collectionCount;
			return;
		}
		else if (index == Count - collectionCount)
		{
			// Removing from the ending: trim the existing view
			Count -= collectionCount;
			return;
		}

		if ((index + (collectionCount / 2)) < Count / 2)
		{
			// Removing from first half of list

			// Move lower items up: [0, index) -> [collectionCount, collectionCount + index)
			var copyCount = index;
			var writeIndex = collectionCount;
			for (var j = copyCount - 1; j != -1; --j)
				_buffer[DequeIndexToBufferIndex(writeIndex + j)] = _buffer[DequeIndexToBufferIndex(j)];

			// Rotate to new view
			PostIncrement(collectionCount);
		}
		else
		{
			// Removing from second half of list

			// Move higher items down: [index + collectionCount, count) -> [index, count - collectionCount)
			var copyCount = Count - collectionCount - index;
			var readIndex = index + collectionCount;
			for (var j = 0; j != copyCount; ++j)
				_buffer[DequeIndexToBufferIndex(index + j)] = _buffer[DequeIndexToBufferIndex(readIndex + j)];
		}

		// Adjust valid count
		Count -= collectionCount;
	}

	/// <summary>
	/// Doubles the capacity if necessary to make room for one more element. When this method returns, <see cref="IsFull" /> is false.
	/// </summary>
	private void EnsureCapacityForOneElement()
	{
		if (IsFull)
		{
			Capacity = (Capacity == 0) ? 1 : Capacity * 2;
		}
	}

	/// <summary>
	/// Inserts a single element at the back of this deque.
	/// </summary>
	/// <param name="value">The element to insert.</param>
	public void AddToBack(T value)
	{
		EnsureCapacityForOneElement();
		DoAddToBack(value);
	}

	/// <summary>
	/// Inserts a single element at the front of this deque.
	/// </summary>
	/// <param name="value">The element to insert.</param>
	public void AddToFront(T value)
	{
		EnsureCapacityForOneElement();
		DoAddToFront(value);
	}

	/// <summary>
	/// Inserts a collection of elements into this deque.
	/// </summary>
	/// <param name="index">The index at which the collection is inserted.</param>
	/// <param name="collection">The collection of elements to insert.</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index to an insertion point for the source.</exception>
	public void InsertRange(int index, IEnumerable<T> collection)
	{
		CheckNewIndexArgument(Count, index);
		var source = collection.Reify();
		var collectionCount = source.Count;

		// Overflow-safe check for "Count + collectionCount > Capacity"
		if (collectionCount > Capacity - Count)
		{
			Capacity = checked(Count + collectionCount);
		}

		if (collectionCount == 0)
		{
			return;
		}

		DoInsertRange(index, source);
	}

	/// <summary>
	/// Removes a range of elements from this deque.
	/// </summary>
	/// <param name="offset">The index into the deque at which the range begins.</param>
	/// <param name="count">The number of elements to remove.</param>
	/// <exception cref="ArgumentOutOfRangeException">Either <paramref name="offset" /> or <paramref name="count" /> is less than 0.</exception>
	/// <exception cref="ArgumentException">The range [<paramref name="offset" />, <paramref name="offset" /> + <paramref name="count" />) is not within the range [0, <see cref="Count" />).</exception>
	public void RemoveRange(int offset, int count)
	{
		CheckRangeArguments(Count, offset, count);

		if (count == 0)
		{
			return;
		}

		DoRemoveRange(offset, count);
	}

	/// <summary>
	/// Removes and returns the last element of this deque.
	/// </summary>
	/// <returns>The former last element.</returns>
	/// <exception cref="InvalidOperationException">The deque is empty.</exception>
	public T RemoveFromBack()
	{
		if (IsEmpty)
			throw new InvalidOperationException("The deque is empty.");

		return DoRemoveFromBack();
	}

	/// <summary>
	/// Removes and returns the first element of this deque.
	/// </summary>
	/// <returns>The former first element.</returns>
	/// <exception cref="InvalidOperationException">The deque is empty.</exception>
	public T RemoveFromFront()
	{
		if (IsEmpty)
		{
			throw new InvalidOperationException("The deque is empty.");
		}

		return DoRemoveFromFront();
	}

	/// <summary>
	/// Removes all items from this deque.
	/// </summary>
	public void Clear()
	{
		_offset = 0;
		Count = 0;
	}

	/// <summary>
	/// Creates and returns a new array containing the elements in this deque.
	/// </summary>
	/// <returns>T[].</returns>
	public T[] ToArray()
	{
		var result = new T[Count];
		((ICollection<T>)this).CopyTo(result, 0);
		return result;
	}
}