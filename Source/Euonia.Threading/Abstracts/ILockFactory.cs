namespace Nerosoft.Euonia.Threading;

/// <summary>
/// Acts as a factory for <see cref="ILockProvider"/> instances of a certain type. This interface may be
/// easier to use than <see cref="ILockProvider"/> in dependency injection scenarios.
/// </summary>
public interface ILockFactory
{
    /// <summary>
    /// Constructs an <see cref="ILockProvider"/> instance with the given <paramref name="name"/>.
    /// </summary>
    ILockProvider Create(string name);
}