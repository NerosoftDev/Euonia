namespace Nerosoft.Euonia.Threading;

/// <summary>
/// Allows synchronization across tasks, regardless of the thread executing it.
/// Doing so involves tracking a state, so that once invalidated, trying to 
/// acquire the lock will fail.
/// </summary>
public class StatefulMutex : IDisposable
{
    private readonly SemaphoreSlim _mutex = new(1, 1);
    private State _state = new();

    /// <summary>
    /// Gets the current state
    /// </summary>
    /// <returns>The current state</returns>
    public State State
    {
        get { return _state; }
    }

    /// <summary>
    /// Advances the current state
    /// </summary>
    /// <returns>The new state</returns>
    public State InvalidateState()
    {
        _state = _state.GetNextState();
        return _state;
    }

    /// <summary>
    /// Checks if the given state is current
    /// </summary>
    /// <param name="state">The state to test</param>
    /// <returns>True if the current state is current, false otherwise</returns>
    public bool IsCurrent(State state)
    {
        return _state.Equals(state);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Context Acquire()
    {
        _mutex.Wait();
        return new Context(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Context Acquire(State state)
    {
        _mutex.Wait();
        if (IsCurrent(state))
        {
            return new Context(this);
        }
        _mutex.Release();
        throw new InvalidOperationException("Cannot lock mutex with expired state");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<Context> AcquireAsync()
    {
        await _mutex.WaitAsync().ConfigureAwait(false);
        return new Context(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Context> AcquireAsync(State state)
    {
        await _mutex.WaitAsync().ConfigureAwait(false);
        if (IsCurrent(state))
        {
            return new Context(this);
        }
        _mutex.Release();
        throw new InvalidOperationException("Cannot lock mutex with expired state");
    }

    /// <summary>
    /// Dispose of the <see cref="StatefulMutex"/> 
    /// </summary>
    public void Dispose() => _mutex.Dispose();

    /// <summary>
    /// The state context of the <see cref="StatefulMutex"/>
    /// </summary>
    public class Context : IDisposable
    {
        private readonly StatefulMutex _parent;

        internal Context(StatefulMutex parent)
        {
            _parent = parent;
        }

        /// <inheritdoc/>
        public void Dispose() => _parent._mutex.Release();
    }
}
