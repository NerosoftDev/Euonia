using System.Reflection;

namespace Nerosoft.Euonia.Reflection;

/// <summary>
/// The methods to perform enumeration operations.
/// </summary>
public static class EnumHelper
{
    /// <summary>
    /// Gets the enum values.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public static TEnum[] GetEnumValues<TEnum>()
    {
        var type = typeof(TEnum);

        if (!type.GetTypeInfo().IsEnum)
        {
            throw new InvalidCastException($"The type {type.FullName} is not enum.");
        }

        return (
            from field in type.GetRuntimeFields() //.GetFields(BindingFlags.Public | BindingFlags.Static)
            where field.IsLiteral
            select (TEnum)field.GetValue(type)).ToArray();
    }

    /// <summary>
    /// Gets the enum names.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static string[] GetEnumNames<TEnum>()
    {
        var type = typeof(TEnum);
        if (!type.GetTypeInfo().IsEnum)
        {
            throw new InvalidCastException($"The type {type.FullName} is not enum.");
        }

        return (
            from field in type.GetRuntimeFields() //.GetFields(BindingFlags.Public | BindingFlags.Static)
            where field.IsLiteral
            select field.Name).ToArray();
    }

    /// <summary>
    /// Gets the first occurrence of the specified type of <see cref="Attribute"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="e"></param>
    /// <returns></returns>
    public static T GetAttribute<T>(Enum e)
        where T : Attribute
    {
        T attribute = default;
        var enumType = e.GetType();
        var members = enumType.GetTypeInfo().DeclaredMembers.ToArray(); //.GetMember(e.ToString());

        if (members.Length == 1)
        {
            var attrs = members[0].GetCustomAttributes(typeof(T), false).ToArray();
            if (attrs.Length > 0)
            {
                attribute = (T)attrs[0];
            }
        }

        {
        }

        return attribute;
    }

    /// <summary>
    /// Gets customer attribute of enum.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T GetCustomerAttribute<T>(Enum value)
        where T : Attribute
    {
        var enumType = value.GetType();
        var name = Enum.GetName(enumType, value);
        if (!string.IsNullOrEmpty(name))
        {
            var fieldInfo = enumType.GetRuntimeField(name);
            if (fieldInfo != null)
            {
                var attr = fieldInfo.GetCustomAttribute<T>();
                if (attr != null)
                {
                    return attr;
                }
            }
        }
        return default;
    }
}
