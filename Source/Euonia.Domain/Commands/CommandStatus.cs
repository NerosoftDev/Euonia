namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The command execution status definition.
/// </summary>
public enum CommandStatus
{
	/// <summary>
	/// Represents the command was successfully executed.
	/// </summary>
	Succeed,

	/// <summary>
	/// Represents the command execution was failed.
	/// </summary>
	Failure,

	/// <summary>
	/// Represents the command execution was canceled.
	/// </summary>
	Canceled,
}