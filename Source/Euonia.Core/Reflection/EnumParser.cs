using System.Reflection;

// ReSharper disable UnusedType.Global

namespace Nerosoft.Euonia.Reflection;

/// <summary>
/// Methods to parse string value to enum type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class EnumParser<T>
{
    private static readonly Dictionary<string, T> _dictionary = new();

    static EnumParser()
    {
        var type = typeof(T);
        if (!type.GetTypeInfo().IsEnum)
        {
            throw new InvalidCastException($"The type {type.FullName} is not enum.");
        }

        var names = Enum.GetNames(type);
        var values = (T[])Enum.GetValues(type);

        for (var i = 0; i < names.Length; i++)
        {
            _dictionary.Add(names[i], values[i]);
        }
    }

    /// <summary>
    /// Try parse string value to enum type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryParse(string name, out T value)
    {
        return _dictionary.TryGetValue(name, out value);
    }

    /// <summary>
    /// Parse string value to enum type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T Parse(string name)
    {
        return _dictionary[name];
    }
}