using System.Linq.Expressions;
using System.Reflection;

namespace Nerosoft.Euonia.Reflection;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public static class PropertyAccessorCache<T> where T : class
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Dictionary<string, LambdaExpression> _cache = new();

    static PropertyAccessorCache()
    {
        var t = typeof(T);
        var parameter = Expression.Parameter(t, "p");
        foreach (var property in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var lambdaExpression = Expression.Lambda(propertyAccess, parameter);
            _cache[property.Name] = lambdaExpression;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static LambdaExpression Get(string propertyName)
    {
        return _cache.TryGetValue(propertyName, out var result) ? result : null;
    }
}
