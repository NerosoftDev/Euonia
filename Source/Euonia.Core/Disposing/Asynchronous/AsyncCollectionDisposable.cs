using System.Collections.Immutable;

namespace Nerosoft.Euonia.Disposing;

/// <summary>
/// Disposes a collection of disposables.
/// </summary>
public sealed class AsyncCollectionDisposable : AsyncSingleDisposable<ImmutableQueue<IAsyncDisposable>>
{
    private readonly DisposeFlags _flags;

    /// <summary>
    /// Creates a disposable that disposes a collection of disposables.
    /// </summary>
    /// <param name="disposables">The disposables to dispose.</param>
    public AsyncCollectionDisposable(params IAsyncDisposable[] disposables)
        : this(disposables, DisposeFlags.ExecuteConcurrently)
    {
    }

    /// <summary>
    /// Creates a disposable that disposes a collection of disposables.
    /// </summary>
    /// <param name="disposables">The disposables to dispose.</param>
    public AsyncCollectionDisposable(IEnumerable<IAsyncDisposable> disposables)
        : this(disposables, DisposeFlags.ExecuteConcurrently)
    {
    }

    /// <summary>
    /// Creates a disposable that disposes a collection of disposables.
    /// </summary>
    /// <param name="disposables">The disposables to dispose.</param>
    /// <param name="flags">Flags that control how asynchronous disposal is handled.</param>
    public AsyncCollectionDisposable(IEnumerable<IAsyncDisposable> disposables, DisposeFlags flags)
        : base(ImmutableQueue.CreateRange(disposables))
    {
        _flags = flags;
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsync(ImmutableQueue<IAsyncDisposable> context)
    {
        if ((_flags & DisposeFlags.ExecuteSerially) == DisposeFlags.ExecuteSerially)
        {
            foreach (var disposable in context)
            {
                await disposable.DisposeAsync().ConfigureAwait(false);
            }
        }
        else
        {
            var tasks = context.Select(disposable => disposable.DisposeAsync().AsTask()).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Adds a disposable to the collection of disposables. If this instance is already disposed or disposing, then <paramref name="disposable"/> is disposed immediately.
    /// </summary>
    /// <param name="disposable">The disposable to add to our collection.</param>
    public ValueTask AddAsync(IAsyncDisposable disposable)
    {
        if (TryUpdateContext(x => x.Enqueue(disposable)))
        {
            return new ValueTask();
        }
        return disposable.DisposeAsync();
    }

    /// <summary>
    /// Creates a disposable that disposes a collection of disposables.
    /// </summary>
    /// <param name="disposables">The disposables to dispose.</param>
    public static CollectionDisposable Create(params IDisposable[] disposables) => new(disposables);

    /// <summary>
    /// Creates a disposable that disposes a collection of disposables.
    /// </summary>
    /// <param name="disposables">The disposables to dispose.</param>
    public static CollectionDisposable Create(IEnumerable<IDisposable> disposables) => new(disposables);
}