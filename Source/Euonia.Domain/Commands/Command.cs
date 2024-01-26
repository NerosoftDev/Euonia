using System.ComponentModel;
using Nerosoft.Euonia.Reflection;

namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The abstract implement of <see cref="ICommand"/>
/// </summary>
public abstract class Command : ICommand
{
	private const string PROPERTY_ID = "nerosoft.euonia.internal.command.id";

	/// <summary>
	/// Initializes a new instance of the <see cref="Command"/> class.
	/// </summary>
	protected Command()
	{
		Properties[PROPERTY_ID] = Guid.NewGuid().ToString();
	}

	/// <summary>
	/// Gets the extended properties of command.
	/// </summary>
	public virtual IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

	/// <summary>
	/// Gets or sets the command property with specified name.
	/// </summary>
	/// <param name="name"></param>
	public virtual string this[string name]
	{
		get => Properties.TryGetValue(name, out var value) ? value : default;
		set => Properties[name] = value;
	}

	/// <summary>
	/// Gets value of property <paramref name="name"/>.
	/// </summary>
	/// <param name="name"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public virtual T GetProperty<T>(string name)
	{
		return TypeHelper.CoerceValue<T, string>(this[name]);
	}

	/// <summary>
	/// Gets or sets the command identifier.
	/// </summary>
	public virtual string CommandId
	{
		get => this[PROPERTY_ID];
		set => this[PROPERTY_ID] = value;
	}
}

/// <inheritdoc />
public abstract class Command<T1> : Command
{
	/// <inheritdoc />
	protected Command()
	{
	}

	/// <inheritdoc />
	protected Command(Tuple<T1> data)
	{
		Data = data;
	}

	/// <inheritdoc />
	protected Command(T1 item1)
		: this(Tuple.Create(item1))
	{
	}

	/// <summary>
	/// Gets or sets the command data.
	/// </summary>
	public Tuple<T1> Data { get; set; }

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	/// <exception cref="IndexOutOfRangeException"></exception>
	public virtual object this[int index]
	{
		get
		{
			return index switch
			{
				0 => Data,
				1 => Data.Item1,
				_ => throw new IndexOutOfRangeException()
			};
		}
	}

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="index"></param>
	/// <returns></returns>
	public virtual T GetItem<T>(int index)
	{
		var value = this[index];
		if (value is T t)
		{
			return t;
		}

		return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
	}

	/// <summary>
	/// Gets the value of the first item.
	/// </summary>
	public T1 Item1 => Data.Item1;
}

/// <inheritdoc />
public abstract class Command<T1, T2> : Command
{
	/// <inheritdoc />
	protected Command()
	{
	}

	/// <inheritdoc />
	protected Command(Tuple<T1, T2> data)
	{
		Data = data;
	}

	/// <inheritdoc />
	protected Command(T1 item1, T2 item2)
		: this(Tuple.Create(item1, item2))
	{
	}

	/// <summary>
	/// Gets or sets the command data.
	/// </summary>
	public Tuple<T1, T2> Data { get; set; }

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	/// <exception cref="IndexOutOfRangeException"></exception>
	public virtual object this[int index]
	{
		get
		{
			return index switch
			{
				0 => Data,
				1 => Data.Item1,
				2 => Data.Item2,
				_ => throw new IndexOutOfRangeException()
			};
		}
	}

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="index"></param>
	/// <returns></returns>
	public virtual T GetItem<T>(int index)
	{
		var value = this[index];
		if (value is T t)
		{
			return t;
		}

		return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
	}

	/// <summary>
	/// Gets the value of the first item.
	/// </summary>
	public T1 Item1 => Data.Item1;

	/// <summary>
	/// Gets the value of the second item.
	/// </summary>
	public T2 Item2 => Data.Item2;
}

/// <inheritdoc />
public abstract class Command<T1, T2, T3> : Command
{
	/// <inheritdoc />
	protected Command()
	{
	}

	/// <inheritdoc />
	protected Command(Tuple<T1, T2, T3> data)
	{
		Data = data;
	}

	/// <inheritdoc />
	protected Command(T1 item1, T2 item2, T3 item3)
		: this(Tuple.Create(item1, item2, item3))
	{
	}

	/// <summary>
	/// Gets or sets the command data.
	/// </summary>
	public Tuple<T1, T2, T3> Data { get; set; }

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	/// <exception cref="IndexOutOfRangeException"></exception>
	public virtual object this[int index]
	{
		get
		{
			return index switch
			{
				0 => Data,
				1 => Data.Item1,
				2 => Data.Item2,
				3 => Data.Item3,
				_ => throw new IndexOutOfRangeException()
			};
		}
	}

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="index"></param>
	/// <returns></returns>
	public T GetItem<T>(int index)
	{
		var value = this[index];
		if (value is T t)
		{
			return t;
		}

		return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
	}

	/// <summary>
	/// Gets the value of the first item.
	/// </summary>
	public T1 Item1 => Data.Item1;

	/// <summary>
	/// Gets the value of the second item.
	/// </summary>
	public T2 Item2 => Data.Item2;

	/// <summary>
	/// Gets the value of the third item.
	/// </summary>
	public T3 Item3 => Data.Item3;
}

/// <inheritdoc />
public abstract class Command<T1, T2, T3, T4> : Command
{
	/// <inheritdoc />
	protected Command()
	{
	}

	/// <inheritdoc />
	protected Command(Tuple<T1, T2, T3, T4> data)
	{
		Data = data;
	}

	/// <inheritdoc />
	protected Command(T1 item1, T2 item2, T3 item3, T4 item4)
		: this(Tuple.Create(item1, item2, item3, item4))
	{
	}

	/// <summary>
	/// Gets or sets the command data.
	/// </summary>
	public Tuple<T1, T2, T3, T4> Data { get; set; }

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	/// <exception cref="IndexOutOfRangeException"></exception>
	public virtual object this[int index]
	{
		get
		{
			return index switch
			{
				0 => Data,
				1 => Data.Item1,
				2 => Data.Item2,
				3 => Data.Item3,
				4 => Data.Item4,
				_ => throw new IndexOutOfRangeException()
			};
		}
	}

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="index"></param>
	/// <returns></returns>
	public virtual T GetItem<T>(int index)
	{
		var value = this[index];
		if (value is T t)
		{
			return t;
		}

		return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
	}

	/// <summary>
	/// Gets the value of the first item.
	/// </summary>
	public T1 Item1 => Data.Item1;

	/// <summary>
	/// Gets the value of the first item.
	/// </summary>
	public T2 Item2 => Data.Item2;

	/// <summary>
	/// Gets the value of the third item.
	/// </summary>
	public T3 Item3 => Data.Item3;

	/// <summary>
	/// Gets the value of the fourth item.
	/// </summary>
	public T4 Item4 => Data.Item4;
}

/// <inheritdoc />
public abstract class Command<T1, T2, T3, T4, T5> : Command
{
	/// <inheritdoc />
	protected Command()
	{
	}

	/// <inheritdoc />
	protected Command(Tuple<T1, T2, T3, T4, T5> data)
	{
		Data = data;
	}

	/// <inheritdoc />
	protected Command(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		: this(Tuple.Create(item1, item2, item3, item4, item5))
	{
	}

	/// <summary>
	/// Gets or sets the command data.
	/// </summary>
	public Tuple<T1, T2, T3, T4, T5> Data { get; set; }

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	/// <exception cref="IndexOutOfRangeException"></exception>
	public virtual object this[int index]
	{
		get
		{
			return index switch
			{
				0 => Data,
				1 => Data.Item1,
				2 => Data.Item2,
				3 => Data.Item3,
				4 => Data.Item4,
				5 => Data.Item5,
				_ => throw new IndexOutOfRangeException()
			};
		}
	}

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="index"></param>
	/// <returns></returns>
	public virtual T GetItem<T>(int index)
	{
		var value = this[index];
		if (value is T t)
		{
			return t;
		}

		return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
	}

	/// <summary>
	/// Gets the value of the first item.
	/// </summary>
	public T1 Item1 => Data.Item1;

	/// <summary>
	/// Gets the value of the first item.
	/// </summary>
	public T2 Item2 => Data.Item2;

	/// <summary>
	/// Gets the value of the third item.
	/// </summary>
	public T3 Item3 => Data.Item3;

	/// <summary>
	/// Gets the value of the fourth item.
	/// </summary>
	public T4 Item4 => Data.Item4;

	/// <summary>
	/// Gets the value of the fifth item.
	/// </summary>
	public T5 Item5 => Data.Item5;
}

/// <inheritdoc />
public abstract class Command<T1, T2, T3, T4, T5, T6> : Command
{
	/// <inheritdoc />
	protected Command()
	{
	}

	/// <inheritdoc />
	protected Command(Tuple<T1, T2, T3, T4, T5, T6> data)
	{
		Data = data;
	}

	/// <inheritdoc />
	protected Command(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
		: this(Tuple.Create(item1, item2, item3, item4, item5, item6))
	{
	}

	/// <summary>
	/// Gets or sets the command data.
	/// </summary>
	public Tuple<T1, T2, T3, T4, T5, T6> Data { get; set; }

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <param name="index"></param>
	/// <exception cref="IndexOutOfRangeException"></exception>
	public virtual object this[int index]
	{
		get
		{
			return index switch
			{
				0 => Data,
				1 => Data.Item1,
				2 => Data.Item2,
				3 => Data.Item3,
				4 => Data.Item4,
				5 => Data.Item5,
				6 => Data.Item6,
				_ => throw new IndexOutOfRangeException()
			};
		}
	}

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <param name="index"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public virtual T GetItem<T>(int index)
	{
		var value = this[index];
		if (value is T t)
		{
			return t;
		}

		return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
	}

	/// <summary>
	/// Gets the value of the first item.
	/// </summary>
	public T1 Item1 => Data.Item1;

	/// <summary>
	/// Gets the value of the first item.
	/// </summary>
	public T2 Item2 => Data.Item2;

	/// <summary>
	/// Gets the value of the third item.
	/// </summary>
	public T3 Item3 => Data.Item3;

	/// <summary>
	/// Gets the value of the fourth item.
	/// </summary>
	public T4 Item4 => Data.Item4;

	/// <summary>
	/// Gets the value of the fifth item.
	/// </summary>
	public T5 Item5 => Data.Item5;

	/// <summary>
	/// Gets the value of the sixth item.
	/// </summary>
	public T6 Item6 => Data.Item6;
}

/// <inheritdoc />
public abstract class Command<T1, T2, T3, T4, T5, T6, T7> : Command
{
	/// <inheritdoc />
	protected Command()
	{
	}

	/// <inheritdoc />
	protected Command(Tuple<T1, T2, T3, T4, T5, T6, T7> data)
	{
		Data = data;
	}

	/// <inheritdoc />
	protected Command(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
		: this(Tuple.Create(item1, item2, item3, item4, item5, item6, item7))
	{
	}

	/// <summary>
	/// Gets or sets the data.
	/// </summary>
	public Tuple<T1, T2, T3, T4, T5, T6, T7> Data { get; set; }

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	/// <exception cref="IndexOutOfRangeException"></exception>
	public virtual object this[int index]
	{
		get
		{
			return index switch
			{
				0 => Data,
				1 => Data.Item1,
				2 => Data.Item2,
				3 => Data.Item3,
				4 => Data.Item4,
				5 => Data.Item5,
				6 => Data.Item6,
				7 => Data.Item7,
				_ => throw new IndexOutOfRangeException()
			};
		}
	}

	/// <summary>
	/// Gets the item value at specified index.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="index"></param>
	/// <returns></returns>
	public virtual T GetItem<T>(int index)
	{
		var value = this[index];
		if (value is T t)
		{
			return t;
		}

		return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
	}

	/// <summary>
	/// Gets the value of the first item.
	/// </summary>
	public T1 Item1 => Data.Item1;

	/// <summary>
	/// Gets the value of the first item.
	/// </summary>
	public T2 Item2 => Data.Item2;

	/// <summary>
	/// Gets the value of the third item.
	/// </summary>
	public T3 Item3 => Data.Item3;

	/// <summary>
	/// Gets the value of the fourth item.
	/// </summary>
	public T4 Item4 => Data.Item4;

	/// <summary>
	/// Gets the value of the fifth item.
	/// </summary>
	public T5 Item5 => Data.Item5;

	/// <summary>
	/// Gets the value of the sixth item.
	/// </summary>
	public T6 Item6 => Data.Item6;

	/// <summary>
	/// Get the value of the seventh item.
	/// </summary>
	public T7 Item7 => Data.Item7;
}