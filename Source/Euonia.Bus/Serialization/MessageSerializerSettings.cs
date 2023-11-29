namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public class MessageSerializerSettings
{
	/// <summary>
	/// Gets or sets a value indicating how to handle reference loops.
	/// </summary>
	public ReferenceLoopStrategy? ReferenceLoop { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to use constructor handling.
	/// </summary>
	public bool UseConstructorHandling { get; set; } = true;

	/// <summary>
	/// Gets or sets the encoding.
	/// </summary>
	public Encoding Encoding { get; set; } = Encoding.UTF8;

	/// <summary>
	/// Defines how to handle reference loop during deserialization.
	/// </summary>
	public enum ReferenceLoopStrategy
	{
		/// <summary>
		/// 
		/// </summary>
		Ignore,

		/// <summary>
		/// 
		/// </summary>
		Preserve,

		/// <summary>
		/// 
		/// </summary>
		Serialize
	}
}