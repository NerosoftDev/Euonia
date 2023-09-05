namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The contract for auditing store.
/// </summary>
public interface IAuditingStore
{
    /// <summary>
    /// Save the <see cref="AuditingRecord"/> to the store.
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    Task SaveAsync(AuditingRecord record);
}