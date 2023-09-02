namespace Nerosoft.Euonia.Repository.Mongo;

/// <summary>
/// The interface used to generate a value.
/// </summary>
public interface IValueGenerator
{
    /// <summary>
    /// Generates a value.
    /// </summary>
    /// <returns></returns>
    object Generate();
}

/// <summary>
/// The interface used to generate a value.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public interface IValueGenerator<out TValue> : IValueGenerator
{
    /// <summary>
    /// Generates a value.
    /// </summary>
    /// <returns></returns>
    new TValue Generate();
}