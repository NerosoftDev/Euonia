using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Nerosoft.Euonia.Repository.EfCore;

/// <summary>
/// Defines conversion for <see cref="DateTime"/> values to universal time.
/// </summary>
public class UniversalTimeConverter : ValueConverter<DateTime, DateTime>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UniversalTimeConverter"/> class.
	/// </summary>
	public UniversalTimeConverter()
		: base(t => ConvertToUniversalTime(t), t => ConvertToLocalTime(t))
	{
	}

	private static DateTime ConvertToUniversalTime(DateTime time)
	{
		return time.Kind switch
		{
			DateTimeKind.Unspecified => DateTime.SpecifyKind(time, DateTimeKind.Local).ToUniversalTime(),
			DateTimeKind.Local => time.ToUniversalTime(),
			DateTimeKind.Utc => time,
			_ => time
		};
	}

	private static DateTime ConvertToLocalTime(DateTime time)
	{
		return time.Kind switch
		{
			DateTimeKind.Unspecified => DateTime.SpecifyKind(time, DateTimeKind.Utc).ToLocalTime(),
			DateTimeKind.Utc => time.ToLocalTime(),
			DateTimeKind.Local => time,
			_ => time
		};
	}
}
