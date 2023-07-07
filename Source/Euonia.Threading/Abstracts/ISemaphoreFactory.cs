namespace Nerosoft.Euonia.Threading;

/// <summary>
/// Acts as a factory for <see cref="ISemaphoreProvider"/> instances of a certain type. This interface may be
/// easier to use than <see cref="ISemaphoreProvider"/> in dependency injection scenarios.
/// </summary>
public interface ISemaphoreFactory
{
    /// <summary>
    /// Constructs an <see cref="ISemaphoreProvider"/> instance with the given <paramref name="name"/>.
    /// </summary>
    ISemaphoreProvider Create(string name, int maxCount);
}