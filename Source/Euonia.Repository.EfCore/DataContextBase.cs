using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Repository.EfCore;

/// <inheritdoc cref="DbContext" />
public abstract class DataContextBase<TContext> : DbContext, IRepositoryContext
	where TContext : DbContext, IRepositoryContext
{
	/// <inheritdoc />
	protected DataContextBase(DbContextOptions<TContext> options)
		: base(options)
	{
		Id = Guid.NewGuid();
	}

	/// <summary>
	/// Gets a value indicate whether the entry values are automatically set or not.
	/// </summary>
	protected abstract bool AutoSetEntryValues { get; }

	/// <summary>
	/// Gets the kind of the date time.
	/// </summary>
	protected virtual DateTimeKind DateTimeKind { get; } = DateTimeKind.Unspecified;

	/// <inheritdoc />
	public override int SaveChanges()
	{
		return SaveChanges(true);
	}

	/// <inheritdoc />
	public override int SaveChanges(bool acceptAllChangesOnSuccess)
	{
		var entries = ChangeTracker.Entries();
		SetEntryValues(entries);
		var result = base.SaveChanges(acceptAllChangesOnSuccess);
		return result;
	}

	#region Implementation of IRepositoryContext

	/// <summary>
	/// 
	/// </summary>
	public Guid Id { get; }

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

	/// <inheritdoc />
	public virtual IEnumerable<object> GetTrackedEntries()
	{
		var entries = ChangeTracker.Entries();

		return entries;
	}

	#endregion

	/// <inheritdoc cref="DbContext.SaveChangesAsync(CancellationToken)" />
	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return await SaveChangesAsync(true, cancellationToken);
	}

	/// <inheritdoc />
	public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
	{
		var entries = ChangeTracker.Entries();
		SetEntryValues(entries);
		var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		return result;
	}

	/// <summary>
	/// Sets the entry values.
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
			var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

			switch (entry.State)
			{
				case EntityState.Added:
					switch (entry.Entity)
					{
						case IHasCreateTime:
							entry.CurrentValues[nameof(IHasCreateTime.CreateTime)] = time;
							break;
						case IHasUpdateTime:
							entry.CurrentValues[nameof(IHasUpdateTime.UpdateTime)] = time;
							break;
						case ITombstone:
							entry.CurrentValues[nameof(ITombstone.IsDeleted)] = false;
							break;
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
				entityType.SetTombstoneQueryFilter();
			}
		}
	}
}