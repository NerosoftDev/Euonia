namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Interface IAsyncTokenProvider
/// </summary>
public interface IAsyncTokenProvider
{
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns>IVolatileToken.</returns>
    IVolatileToken GetToken(Action<Action<IVolatileToken>> task);
}