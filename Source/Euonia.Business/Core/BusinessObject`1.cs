using System.Linq.Expressions;
using Nerosoft.Euonia.Reflection;

namespace Nerosoft.Euonia.Business;

/// <summary>
/// Abstract class that serves as the base for all business objects.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BusinessObject<T> : BusinessObject
    where T : BusinessObject<T>
{
    /// <summary>
    /// Registers a property.
    /// </summary>
    /// <typeparam name="TValue">The property value.</typeparam>
    /// <param name="info"></param>
    /// <returns></returns>
    protected static PropertyInfo<TValue> RegisterProperty<TValue>(PropertyInfo<TValue> info)
    {
        return PropertyInfoManager.RegisterProperty(typeof(T), info);
    }

    /// <summary>
    /// Registers a property.
    /// </summary>
    /// <typeparam name="TValue">The property value.</typeparam>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    protected static PropertyInfo<TValue> RegisterProperty<TValue>(string propertyName)
    {
        return RegisterProperty(new PropertyInfo<TValue>(propertyName, typeof(T)));
    }

    /// <summary>
    /// Registers a property.
    /// </summary>
    /// <typeparam name="TValue">The property value.</typeparam>
    /// <param name="expression"></param>
    /// <returns></returns>
    protected static PropertyInfo<TValue> RegisterProperty<TValue>(Expression<Func<T, object>> expression)
    {
        var property = Reflect<T>.GetProperty(expression);
        return RegisterProperty<TValue>(property.Name);
    }

    /// <summary>
    /// Registers a property.
    /// </summary>
    /// <typeparam name="TValue">The property value.</typeparam>
    /// <param name="propertyName"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    protected static PropertyInfo<TValue> RegisterProperty<TValue>(string propertyName, TValue defaultValue)
    {
        var property = new PropertyInfo<TValue>(propertyName, typeof(T), defaultValue);
        return RegisterProperty(property);
    }

    /// <summary>
    /// Registers a property.
    /// </summary>
    /// <typeparam name="TValue">The property value.</typeparam>
    /// <param name="expression"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    protected static PropertyInfo<TValue> RegisterProperty<TValue>(Expression<Func<T, object>> expression, TValue defaultValue)
    {
        var property = Reflect<T>.GetProperty(expression);
        return RegisterProperty(property.Name, defaultValue);
    }
}