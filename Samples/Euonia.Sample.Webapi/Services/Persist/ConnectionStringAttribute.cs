namespace Nerosoft.Euonia.Sample.Persist;

/// <summary>
/// Indicates that a class is associated with a specific connection string.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
internal class ConnectionStringAttribute : Attribute
{
	/// <summary>
	/// Gets or sets the connection name.
	/// </summary>
	/// <remarks>
	/// This name is used to identify the connection string in configuration files.
	/// </remarks>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the connection string value.
	/// </summary>
	public string Value { get; set; }
}
