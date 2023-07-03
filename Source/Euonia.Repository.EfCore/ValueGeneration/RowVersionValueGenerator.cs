using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Nerosoft.Euonia.Repository.EfCore;

/// <inheritdoc />
public class RowVersionValueGenerator : ValueGenerator<byte[]>
{
    /// <inheritdoc />
    public override byte[] Next(EntityEntry entry)
    {
        var ticks = DateTime.UtcNow.Ticks;
        return BitConverter.GetBytes(ticks);
    }

    /// <inheritdoc />
    public override bool GeneratesTemporaryValues => false;
}