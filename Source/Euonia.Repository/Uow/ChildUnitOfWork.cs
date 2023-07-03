﻿namespace Nerosoft.Euonia.Repository;

internal class ChildUnitOfWork : UnitOfWorkBase, IUnitOfWork
{
    public event EventHandler<UnitOfWorkEventArgs> Completed;
    public event EventHandler<UnitOfWorkFailedEventArgs> Failed;

    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public Dictionary<string, object> Items => _parent.Items;

    public IReadOnlyDictionary<Type, IRepositoryContext> Contexts => _parent.Contexts;

    public override IServiceProvider ServiceProvider => _parent.ServiceProvider;

    public IUnitOfWork Outer => _parent.Outer;
    public bool IsReserved => _parent.IsReserved;
    public bool IsDisposed => _parent.IsDisposed;
    public bool IsCompleted => _parent.IsCompleted;

    public string ReservationName => _parent.ReservationName;

    private readonly IUnitOfWork _parent;

    public ChildUnitOfWork(IUnitOfWork parent)
    {
        Check.EnsureNotNull(parent, nameof(parent));

        _parent = parent;

        _parent.Failed += (sender, args) =>
        {
            Failed?.Invoke(sender, args);
        };
        _parent.Disposed += InvokeDisposedEvent;
        _parent.Completed += (sender, args) =>
        {
            Completed?.Invoke(sender, args);
        };
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _parent.SaveChangesAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _parent.RollbackAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    public void OnCompleted(Func<Task> handler)
    {
        _parent.OnCompleted(handler);
    }

    public void SetOuter(IUnitOfWork outer)
    {
        _parent.SetOuter(outer);
    }

    public void Initialize(UnitOfWorkOptions options)
    {
        _parent.Initialize(options);
    }

    public void Reserve(string reservationName)
    {
        _parent.Reserve(reservationName);
    }

    public TContext CreateContext<TContext>(string connectionString) where TContext : IRepositoryContext
    {
        return _parent.CreateContext<TContext>(connectionString);
    }

    public TContext CreateContext<TContext>() where TContext : IRepositoryContext
    {
        return _parent.CreateContext<TContext>();
    }

    protected override void Dispose(bool disposing)
    {
    }
}