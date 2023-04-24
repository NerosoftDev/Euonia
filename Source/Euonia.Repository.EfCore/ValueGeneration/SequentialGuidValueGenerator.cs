using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Nerosoft.Euonia.Repository.EfCore;

public class SequentialGuidValueGenerator : ValueGenerator<Guid>
{
    /// <inheritdoc />
    public override bool GeneratesTemporaryValues => false;

    /// <inheritdoc />
    public override Guid Next(EntityEntry entry)
    {
        return ObjectId.NewGuid(GuidType.SequentialAsString);
    }
}