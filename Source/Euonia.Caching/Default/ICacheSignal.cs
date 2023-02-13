namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Interface ISignals
/// Implements the <see cref="IVolatileProvider" />
/// </summary>
/// <seealso cref="IVolatileProvider" />
public interface ICacheSignal : IVolatileProvider
{
    /// <summary>
    /// Triggers the specified signal.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="signal">The signal.</param>
    void Trigger<T>(T signal);

    /// <summary>
    /// Whens the specified signal.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="signal">The signal.</param>
    /// <returns>IVolatileToken.</returns>
    IVolatileToken When<T>(T signal);
}