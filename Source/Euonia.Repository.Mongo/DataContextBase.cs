using System.Data;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Repository.Mongo;

public abstract class DataContextBase<TContext> : MongoDbContext, IRepositoryContext
    where TContext : MongoDbContext, IRepositoryContext
{
    private readonly ILogger<TContext> _logger;

    protected DataContextBase(IMongoDatabase database, ILoggerFactory logger)
        : base(database)
    {
        Session = Database.Client.StartSession();
        _logger = logger.CreateLogger<TContext>();
        //using (var cursor = database.Watch())
        //{
        //    foreach (var change in cursor.ToEnumerable())
        //    {
        //        change.
        //    }
        //}
    }

    public void Dispose()
    {
        Session.Dispose();
        GC.SuppressFinalize(this);
    }

    protected abstract bool EnabledPublishEvents { get; }
    
    protected IClientSessionHandle Session { get; }

    /// <summary>
    /// 
    /// </summary>
    public Guid Id => Guid.NewGuid();

    /// <summary>
    /// 
    /// </summary>
    public virtual string Provider => "MongoDB";

    public IQueryable<TEntity> SetOf<TEntity>()
        where TEntity : class
    {
        return Collection<TEntity>().AsQueryable();
    }

    public IDbConnection GetConnection()
    {
        throw new NotSupportedException();
    }

    public IDbTransaction GetTransaction()
    {
        throw new NotSupportedException();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(0);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }
    
    protected virtual void RaiseDomainEvents(IEnumerable<DomainEvent> events)
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
            _logger.LogError(exception, "RaiseDomainEvents Error");
            Console.WriteLine(exception);
        }
    }

    protected abstract Task PublishEventAsync<TEvent>(TEvent @event)
        where TEvent : DomainEvent;
}