using System.Linq.Expressions;
using Nerosoft.Euonia.Reflection;

namespace Nerosoft.Euonia.Business;

public abstract class BusinessObject<T> : BusinessObject
    where T : BusinessObject<T>
{
    protected static PropertyInfo<TValue> RegisterProperty<TValue>(PropertyInfo<TValue> info)
    {
        return PropertyInfoManager.RegisterProperty(typeof(T), info);
    }

    protected static PropertyInfo<TValue> RegisterProperty<TValue>(string propertyName)
    {
        return RegisterProperty(new PropertyInfo<TValue>(propertyName, typeof(T)));
    }

    protected static PropertyInfo<TValue> RegisterProperty<TValue>(Expression<Func<T, object>> expression)
    {
        var property = Reflect<T>.GetProperty(expression);
        return RegisterProperty<TValue>(property.Name);
    }

    protected static PropertyInfo<TValue> RegisterProperty<TValue>(string propertyName, TValue defaultValue)
    {
        var property = new PropertyInfo<TValue>(propertyName, typeof(T), defaultValue);
        return RegisterProperty(property);
    }

    protected static PropertyInfo<TValue> RegisterProperty<TValue>(Expression<Func<T, object>> expression, TValue defaultValue)
    {
        var property = Reflect<T>.GetProperty(expression);
        return RegisterProperty(property.Name, defaultValue);
    }
}