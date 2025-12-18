using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Nerosoft.Euonia.Sample.Persist;

/// <summary>
/// Generates the current UTC time.
/// </summary>
internal class UtcTimeValueGenerator : ValueGenerator<DateTime>
{
	public override DateTime Next(EntityEntry entry)
	{
		return DateTime.UtcNow;
	}

	public override bool GeneratesTemporaryValues => false;
}
