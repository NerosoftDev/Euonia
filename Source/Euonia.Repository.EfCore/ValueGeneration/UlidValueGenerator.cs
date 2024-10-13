using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Nerosoft.Euonia.Repository.EfCore;

/// <summary>
/// The ULID (Universally Unique Lexicographically Sortable Identifier) value generator.
/// </summary>
public class UlidValueGenerator : ValueGenerator<string>
{
	/// <inheritdoc />
	public override string Next(EntityEntry entry)
	{
		return ObjectId.NewUlid();
	}

	/// <inheritdoc />
	public override bool GeneratesTemporaryValues => false;
}