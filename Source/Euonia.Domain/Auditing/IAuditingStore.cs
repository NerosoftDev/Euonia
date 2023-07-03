namespace Nerosoft.Euonia.Domain;

public interface IAuditingStore
{
    Task SaveAsync(AuditingRecord record);
}