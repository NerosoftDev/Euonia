using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Repository.EfCore;

/// <inheritdoc cref="DbContext" />
public abstract class DataContextBase<TContext> : DbContext, IRepositoryContext
    where TContext : DbContext, IRepositoryContext
{
    private readonly ILogger<TContext> _logger;

    /// <inheritdoc />
    protected DataContextBase(DbContextOptions<TContext> options, ILoggerFactory factory)
        : base(options)
    {
        _logger = factory.CreateLogger<TContext>();
    }

    /// <summary>
    /// 
    /// </summary>
    protected abstract bool AutoSetEntryValues { get; }

    /// <summary>
    /// Gets a value indicate whether the domain events publishing are enabled or not.
    /// </summary>
    protected abstract bool EnabledPublishEvents { get; }

    /// <inheritdoc />
    public override int SaveChanges()
    {
        return SaveChanges(true);
    }

    /// <inheritdoc />
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var entries = ChangeTracker.Entries();
        var events = GetTrackedEvents(entries);
        SetEntryValues(entries);
        var result = base.SaveChanges(acceptAllChangesOnSuccess);
        if (result > 0 && events.Any())
        {
            PublishEvents(events);
        }

        return result;
    }

    #region Implementation of IRepositoryContext

    /// <summary>
    /// 
    /// </summary>
    public Guid Id => Guid.NewGuid();

    /// <summary>
    /// 
    /// </summary>
    public virtual string Provider => Database.ProviderName;

    /// <inheritdoc />
    public IQueryable<TEntity> SetOf<TEntity>()
        where TEntity : class
    {
        return Set<TEntity>();
    }

    /// <inheritdoc />
    public IDbConnection GetConnection()
    {
        return Database.GetDbConnection();
    }

    /// <inheritdoc />
    public IDbTransaction GetTransaction()
    {
        return Database.CurrentTransaction?.GetDbTransaction();
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await Database.RollbackTransactionAsync(cancellationToken);
    }

    #endregion

    /// <inheritdoc cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)" />
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(true, cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries();
        var events = GetTrackedEvents(entries);
        SetEntryValues(entries);
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        if (result > 0 && events.Any())
        {
            PublishEvents(events);
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entries"></param>
    /// <returns></returns>
    protected virtual IEnumerable<DomainEvent> GetTrackedEvents(IEnumerable<EntityEntry> entries)
    {
        var events = new List<DomainEvent>();

        foreach (var entry in entries)
        {
            if (entry.Entity is not IHasDomainEvents aggregate)
            {
                continue;
            }

            aggregate.AttachToEvents();
            events.AddRange(aggregate.GetEvents());
            aggregate.ClearEvents();
        }

        return events;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entries"></param>
    protected virtual void SetEntryValues(IEnumerable<EntityEntry> entries)
    {
        if (!AutoSetEntryValues)
        {
            return;
        }

        foreach (var entry in entries)
        {
            var time = DateTime.UtcNow;

            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity is IHasCreateTime)
                    {
                        entry.CurrentValues[nameof(IHasCreateTime.CreateTime)] = time;
                    }

                    if (entry.Entity is IHasUpdateTime)
                    {
                        entry.CurrentValues[nameof(IHasUpdateTime.UpdateTime)] = time;
                    }

                    if (entry.Entity is ITombstone)
                    {
                        entry.CurrentValues[nameof(ITombstone.IsDeleted)] = false;
                    }

                    break;
                case EntityState.Deleted:
                    if (entry.Entity is ITombstone)
                    {
                        entry.State = EntityState.Modified;
                        entry.CurrentValues[nameof(ITombstone.IsDeleted)] = true;
                        entry.CurrentValues[nameof(ITombstone.DeleteTime)] = time;
                    }

                    break;
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Modified:
                    SetModifiedEntry(entry, time);

                    break;
                default:
                    continue;
            }
        }
    }

    private static void SetModifiedEntry(EntityEntry entry, DateTime time)
    {
        if (entry.State != EntityState.Modified)
        {
            return;
        }

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (entry.Entity is ITombstone { IsDeleted: true })
        {
            entry.CurrentValues[nameof(ITombstone.IsDeleted)] = true;
            entry.CurrentValues[nameof(ITombstone.DeleteTime)] = time;
            return;
        }

        if (entry.Entity is IHasUpdateTime)
        {
            entry.CurrentValues[nameof(IHasUpdateTime.UpdateTime)] = time;
        }
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            //other automated configurations left out
            if (typeof(ITombstone).IsAssignableFrom(entityType.ClrType))
            {
                entityType.SetSoftDeleteQueryFilter();
            }
        }
    }

    /// <summary>
    /// Publishes the domain events.
    /// </summary>
    /// <param name="events"></param>
    protected virtual void PublishEvents(IEnumerable<DomainEvent> events)
    {
        if (!EnabledPublishEvents)
        {
            return;
        }

        try
        {
            async void Action(DomainEvent @event)
            {
                await PublishEventAsync(@event);
            }

            Parallel.ForEach(events, Action);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "PublishEvents Error");
            Console.WriteLine(exception);
        }
    }

    /// <summary>
    /// Publishes the domain event asynchronously.
    /// </summary>
    /// <param name="event"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    protected abstract Task PublishEventAsync<TEvent>(TEvent @event)
        where TEvent : DomainEvent;
}