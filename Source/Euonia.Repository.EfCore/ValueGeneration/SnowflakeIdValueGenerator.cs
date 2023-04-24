using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Nerosoft.Euonia.Repository.EfCore;

/// <inheritdoc />
public class SnowflakeIdValueGenerator : ValueGenerator<long>
{
    /// <inheritdoc />
    public override bool GeneratesTemporaryValues => false;

    /// <inheritdoc />
    public override long Next(EntityEntry entry)
    {
        var id = ObjectId.NewSnowflake();
        return id;
    }
}