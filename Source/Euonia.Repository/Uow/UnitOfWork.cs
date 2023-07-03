using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Nerosoft.Euonia.Repository;

/// <summary>
/// The abstract implement of <see cref="IUnitOfWork"/>.
/// </summary>
public sealed class UnitOfWork : UnitOfWorkBase, IUnitOfWork
{
    #region Events

    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<UnitOfWorkEventArgs> Completed;

    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<UnitOfWorkFailedEventArgs> Failed;

    #endregion

    #region Fields

    private readonly UnitOfWorkOptions _defaultOptions;

    private readonly Dictionary<Type, IRepositoryContext> _contexts = new();

    #endregion

    #region Ctors

    /// <summary>
    /// Initialize a new instance of <see cref="UnitOfWork"/>.
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="options"></param>
    public UnitOfWork(IServiceScopeFactory factory, IOptionsMonitor<UnitOfWorkOptions> options)
    {
        ServiceProvider = factory.CreateScope().ServiceProvider;
        _defaultOptions = options.CurrentValue;
    }

    #endregion

    #region Properties of IUnitOfWork

    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public Dictionary<string, object> Items { get; } = new();

    /// <inheritdoc />
    public IReadOnlyDictionary<Type, IRepositoryContext> Contexts => _contexts;

    /// <summary>
    /// 
    /// </summary>
    public override IServiceProvider ServiceProvider { get; }

    /// <inheritdoc />
    public IUnitOfWork Outer { get; private set; }

    /// <inheritdoc />
    public bool IsReserved { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ReservationName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsCompleted { get; private set; }

    #endregion

    #region Properties of self

    private List<Func<Task>> CompletedHandlers { get; } = new();

    /// <summary>
    /// 
    /// </summary>
    public UnitOfWorkOptions Options { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public Exception Exception { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public bool RolledBack { get; private set; }

    #endregion

    #region Methods of IUnitOfWork

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="handler"></param>
    public void OnCompleted(Func<Task> handler)
    {
        CompletedHandlers.Add(handler);
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="options"></param>
    public void Initialize(UnitOfWorkOptions options)
    {
        Check.EnsureNotNull(options, nameof(options));

        if (Options != null)
        {
            throw new Exception("This unit of work is already initialized before!");
        }

        Options = _defaultOptions.Normalize(options);

        IsReserved = false;
    }

    /// <inheritdoc />
    public void Reserve(string reservationName)
    {
        Check.EnsureNotNull(reservationName, nameof(reservationName));

        ReservationName = reservationName;
        IsReserved = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="outer"></param>
    public void SetOuter(IUnitOfWork outer)
    {
        Outer = outer;
    }

    /// <summary>
    /// Commit the changes.
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (RolledBack)
        {
            return;
        }

        try
        {
            await SaveChangesAsync(cancellationToken);
            IsCompleted = true;
            await OnCompletedAsync();
        }
        catch (Exception exception)
        {
            Exception = exception;
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var (_, context) in Contexts)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task OnCompletedAsync()
    {
        foreach (var handler in CompletedHandlers)
        {
            await handler.Invoke();
        }

        Completed?.Invoke(this, new UnitOfWorkEventArgs(this));
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (RolledBack)
        {
            return;
        }

        foreach (var (_, context) in Contexts)
        {
            await context.RollbackAsync(cancellationToken);
        }

        RolledBack = true;
    }

    /// <inheritdoc />
    public TContext CreateContext<TContext>(string connectionString)
        where TContext : IRepositoryContext
    {
        var context = ServiceProvider.GetRequiredService<TContext>();
        _contexts[typeof(TContext)] = context;
        return context;
    }

    /// <inheritdoc />
    public TContext CreateContext<TContext>()
        where TContext : IRepositoryContext
    {
        var context = ServiceProvider.GetRequiredService<TContext>();
        _contexts[typeof(TContext)] = context;
        return context;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;

        foreach (var (_, _) in Contexts)
        {
            //context.Dispose();
        }

        if (!IsCompleted || Exception != null)
        {
            OnFailed();
        }

        InvokeDisposedEvent(this, new DisposedEventArgs());
    }

    #endregion

    #region Methods of self

    private void OnFailed()
    {
        Failed?.Invoke(this, new UnitOfWorkFailedEventArgs(this, Exception, RolledBack));
    }

    #endregion
}