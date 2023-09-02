using System.Reflection;

namespace Nerosoft.Euonia.Business;

/// <summary>
/// The <see cref="PropertyInfo{T}"/> class provides a strongly-typed wrapper for a <see cref="PropertyInfo"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PropertyInfo<T> : IPropertyInfo
{
	/// <inheritdoc />
	public PropertyInfo(string name)
		: this(name, null)
	{
		Name = name;
	}

	/// <inheritdoc />
	public PropertyInfo(string name, T defaultValue)
		: this(name, null, defaultValue)
	{
	}

	/// <inheritdoc />
	public PropertyInfo(string name, Type objectType)
		: this(name, objectType, GetDefaultValue())
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PropertyInfo{T}"/> class.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="objectType"></param>
	/// <param name="defaultValue"></param>
	public PropertyInfo(string name, Type objectType, T defaultValue)
	{
		Name = name;
		_propertyInfo = objectType?.GetProperty(name);
		DefaultValue = defaultValue;
	}

	/// <inheritdoc />
	public string Name { get; }

	/// <inheritdoc />
	public int CompareTo(object obj)
	{
		return string.Compare(Name, (((IPropertyInfo)obj)!).Name, StringComparison.InvariantCulture);
	}

	/// <inheritdoc />
	public Type Type => typeof(T);

	/// <summary>
	/// Gets the default initial value for the property.
	/// </summary>
	public virtual T DefaultValue { get; }

	object IPropertyInfo.DefaultValue => DefaultValue;

	private readonly PropertyInfo _propertyInfo;

	/// <inheritdoc />
	public PropertyInfo GetPropertyInfo() => _propertyInfo;

	/// <summary>
	/// Gets the default initial value for the property.
	/// </summary>
	/// <returns></returns>
	public static T GetDefaultValue()
	{
		// if T is string we need an empty string, not null, for data binding
		if (typeof(T) == typeof(string))
		{
			return (T)(object)string.Empty;
		}

		return default;
	}

	IFieldData IPropertyInfo.NewFieldData(string name)
	{
		return NewFieldData(name);
	}

	/// <summary>
	/// Gets a new <see cref="IFieldData"/> instance with specified name.
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	protected virtual IFieldData NewFieldData(string name)
	{
		return new FieldData<T>(name);
	}
}