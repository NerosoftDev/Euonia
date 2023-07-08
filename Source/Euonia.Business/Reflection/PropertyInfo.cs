using System.Reflection;

namespace Nerosoft.Euonia.Business;

public class PropertyInfo<T> : IPropertyInfo
{
    public PropertyInfo(string name)
        : this(name, null)
    {
        Name = name;
    }

    public PropertyInfo(string name, T defaultValue)
        : this(name, null, defaultValue)
    {
    }

    public PropertyInfo(string name, Type objectType)
        : this(name, objectType, GetDefaultValue())
    {
    }

    public PropertyInfo(string name, Type objectType, T defaultValue)
    {
        Name = name;
        _propertyInfo = objectType?.GetProperty(name);
        DefaultValue = defaultValue;
    }

    public string Name { get; }

    public int CompareTo(object obj)
    {
        return string.Compare(Name, ((IPropertyInfo)obj).Name, StringComparison.InvariantCulture);
    }

    public Type Type => typeof(T);

    /// <summary>
    /// Gets the default initial value for the property.
    /// </summary>
    public virtual T DefaultValue { get; }

    object IPropertyInfo.DefaultValue => DefaultValue;

    private readonly PropertyInfo _propertyInfo;
    public PropertyInfo GetPropertyInfo() => _propertyInfo;

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

    protected virtual IFieldData NewFieldData(string name)
    {
        return new FieldData<T>(name);
    }
}